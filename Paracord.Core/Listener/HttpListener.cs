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
        /// The SSL certificate to use for HTTPS connections, if enabled.
        /// </summary>
        protected readonly X509Certificate2? SslCertificate;

        /// <summary>
        /// Whether the listener is secured with HTTPS.
        /// </summary>
        protected bool IsSecure
        {
            get => this.SslCertificate != null;
        }

        /// <summary>
        /// Initialize a new <see cref="HttpListener" />-instance with the specified IP-address and port.
        /// </summary>
        /// <param name="address">The IP-address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="sslCertificate">The SSL certificate to use for HTTPS connections. HTTPS is disabled, if null.</param>
        public HttpListener(string address = "*", UInt16 port = 80, X509Certificate2? sslCertificate = null) : base(address, port)
        {
            this.RegisterMiddleware<DateMiddleware>();
            this.RegisterMiddleware<ServerMiddleware>();
            this.RegisterMiddleware<CompressionMiddleware>();

            this.RegisterCompression<GzipCompressionProvider>();

            this.SslCertificate = sslCertificate;
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
        /// Accept a new HTTP request from the listener, synchronously.
        /// This method will block until a request is received.
        /// </summary>
        /// <returns>The received <see cref="HttpContext" />-instance.</returns>
        public HttpContext AcceptRequest()
            => this.WrapTcpClient(this.AcceptClient());

        /// <summary>
        /// Accept a new HTTP request from the listener, asynchronously.
        /// This method will block until a request is received.
        /// </summary>
        /// <returns>A task, resolving to the received <see cref="HttpContext" />-instance.</returns>
        public async Task<HttpContext> AcceptRequestAsync()
            => this.WrapTcpClient(await this.AcceptClientAsync());

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

        /// <summary>
        /// Parse the content from a <c>TcpClient</c>-instance into an HTTP context object.
        /// </summary>
        /// <param name="client">The <see cref="TcpClient" />-instance to parse from.</param>
        /// <returns>The parsed <see cref="HttpContext" />-object.</returns>
        protected HttpContext WrapTcpClient(TcpClient client)
        {
            Stream stream = client.GetStream();
            HttpContext ctx = new HttpContext(this, client);

            if(this.IsSecure)
            {
                stream = this.CreateSslStream(stream);
            }

            // Reading client.Available will reset it
            int bytesExpected = 0;
            int bytesRead = 0;

            // Wait until data is available
            while((bytesExpected = client.Available) == 0) { }

            Byte[] bytes = new Byte[2048];
            bytesRead = stream.Read(bytes, 0, bytes.Length);

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