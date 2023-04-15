using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using Paracord.Core.Compression;
using Paracord.Core.Http;
using Paracord.Core.Middleware;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Listener
{
    public class HttpListener : ListenerBase
    {
        /// <summary>
        /// List of all middlewares used by the listener.
        /// </summary>
        protected Dictionary<string, MiddlewareBase> Middlewares { get; set; } = new();

        /// <summary>
        /// Compression provider collections used by the listener.
        /// </summary>
        public readonly CompressionProviderCollection CompressionProviders = new();

        /// <summary>
        /// Initialize a new <see cref="HttpListener" />-instance.
        /// </summary>
        /// <param name="sslCertificate">The SSL certificate to use for HTTPS connections. HTTPS is disabled, if null.</param>
        public HttpListener(X509Certificate2? sslCertificate = null) : base(sslCertificate)
        {
            this.RegisterMiddleware<DateMiddleware>();
            this.RegisterMiddleware<ServerMiddleware>();
            this.RegisterMiddleware<CompressionMiddleware>();
            this.RegisterMiddleware<ETagMiddleware>();

            this.RegisterCompression<BrotliCompressionProvider>();
            this.RegisterCompression<DeflateCompressionProvider>();
            this.RegisterCompression<GzipCompressionProvider>();
        }

        /// <inheritdoc cref="ListenerBase.Start" />
        public override void Start()
        {
            base.Start();

            this.ExecuteMiddleware(_ => _.OnServerStarted(this));
        }

        /// <inheritdoc cref="ListenerBase.Stop" />
        public override void Stop()
        {
            this.ExecuteMiddleware(_ => _.OnServerClosed(this));

            base.Stop();
        }

        /// <summary>
        /// Start receiving connections and pass them to the specified <paramref name="executor" />.
        /// This method is blocking, while the listener is running.
        /// </summary>
        /// <param name="executor">The action for handling incoming HTTP requests.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken" /> for halting.</param>
        public void AcceptConnections(Action<HttpContext> executor, CancellationToken cancellationToken = default!)
        {
            List<Task> listenerTasks = new List<Task>();

            foreach(TcpListener listener in this.Listeners)
            {
                Task listenerTask = Task.Run(async () =>
                {
                    await this.InternalListenerProc(listener, executor, cancellationToken);
                });
                listenerTasks.Add(listenerTask);
            }

            Task.WaitAll(listenerTasks.ToArray());
        }

        /// <summary>
        /// Register a new middleware onto the listener to handle internal actions.
        /// </summary>
        /// <param name="middleware">The middleware to register.</param>
        /// <typeparam name="T">The type of middleware to register.</typeparam>
        public void RegisterMiddleware<T>(T middleware) where T : MiddlewareBase
            => this.Middlewares.Add(typeof(T).FullName ?? typeof(T).Name, middleware);

        /// <inheritdoc cref="HttpListener.RegisterMiddleware{T}" />
        public void RegisterMiddleware<T>() where T : MiddlewareBase, new()
            => this.RegisterMiddleware(new T());

        /// <summary>
        /// Register a new compression provider onto the listener to handle internal actions.
        /// </summary>
        /// <param name="provider">The provider to register.</param>
        /// <typeparam name="T">The type of middleware to register.</typeparam>
        public void RegisterCompression<T>(T provider) where T : ICompressionProvider
            => this.CompressionProviders.Add(provider);

        /// <inheritdoc cref="HttpListener.RegisterCompression{T}" />
        public void RegisterCompression<T>() where T : ICompressionProvider, new()
            => this.RegisterCompression(new T());

        protected async Task InternalListenerProc(TcpListener listener, Action<HttpContext> executor, CancellationToken cancellationToken = default!)
        {
            while(this.IsOpen)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);

                Task _ = Task.Run(() =>
                {
                    HttpContext context = this.WrapTcpClient(client);
                    executor(context);
                });
            }
        }

        /// <summary>
        /// Parse the content from a <c>TcpClient</c>-instance into an HTTP context object.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient" />-instance to parse from.</param>
        /// <returns>The parsed <see cref="HttpContext" />-object.</returns>
        protected HttpContext WrapTcpClient(TcpClient client)
        {
            HttpContext ctx = new HttpContext(this, client);

            int bytesExpected = 0;
            int bytesRead = 0;

            // Reading client.Available will reset it
            while((bytesExpected = client.Available) == 0) { }

            Byte[] bytes = new Byte[2048];
            bytesRead = ctx.ConnectionStream.Read(bytes, 0, bytes.Length);

            // Limit bytes size
            bytes = bytes[0..bytesRead];

            ctx.Request = new HttpRequest();
            ctx.Request.Context = ctx;

            HttpParser.DeserializeRequest(ctx.Request, bytes);

            ctx.Response = new HttpResponse();
            ctx.Response.Context = ctx;

            this.ExecuteMiddleware(_ => _.AfterRequestReceived(this, ctx.Request, ctx.Response));

            return ctx;
        }

        /// <summary>
        /// Execute a middleware method, selected by the specified selector.
        /// </summary>
        /// <param name="runner">Selects the method to run on the middlewares.</param>
        /// <param name="reversed">Whether to reverse the middleware ordering.</param>
        /// <example>
        /// For example, to execute the <see cref="MiddlewareBase.OnServerStarted(HttpListener)" />-method in all the middleware:
        /// <code>
        /// this.ExecuteMiddleware(_ => _.OnServerStarted(this));
        /// </code>
        /// </example>
        internal void ExecuteMiddleware(Action<MiddlewareBase> runner, bool reversed = false)
        {
            var middlewares = this.Middlewares;

            // Useful for response-middleware.
            if(reversed)
            {
                middlewares.Reverse();
            }

            foreach(var middleware in this.Middlewares)
            {
                runner(middleware.Value);

                if(!middleware.Value.UseNextMiddleware)
                {
                    break;
                }
            }
        }
    }
}