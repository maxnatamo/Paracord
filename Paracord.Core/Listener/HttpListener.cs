using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using Paracord.Core.Http;
using Paracord.Shared.Models.Http;
using Paracord.Shared.Models.Listener;

namespace Paracord.Core.Listener
{
    public class HttpListener : ListenerBase
    {
        /// <summary>
        /// Initialize a new <see cref="HttpListener" />-instance.
        /// </summary>
        /// <param name="sslCertificate">The SSL certificate to use for HTTPS connections. HTTPS is disabled, if null.</param>
        public HttpListener(X509Certificate2? sslCertificate = null) : base(sslCertificate)
        {
            
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

            foreach(KeyValuePair<ListenerPrefix, TcpListener> kv in this.Listeners)
            {
                Task listenerTask = Task.Run(async () =>
                {
                    await this.InternalListenerProc(
                        listener: kv.Value,
                        listenerPrefix: kv.Key,
                        executor: executor,
                        cancellationToken: cancellationToken);
                });

                listenerTasks.Add(listenerTask);
            }

            Task.WaitAll(listenerTasks.ToArray());
        }

        /// <summary>
        /// Internal process for handling a single listener of the <see cref="ListenerBase.Listeners" />.
        /// </summary>
        /// <param name="listener">The listener which the process should execute with.</param>
        /// <param name="executor">The underlying handler for HTTP requests.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken" />, for halting the process.</param>
        protected async Task InternalListenerProc(TcpListener listener, ListenerPrefix listenerPrefix, Action<HttpContext> executor, CancellationToken cancellationToken = default!)
        {
            while(this.IsOpen && !cancellationToken.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);

                Task _ = Task.Run(() =>
                {
                    HttpContext context = this.WrapTcpClient(listenerPrefix, client);
                    executor(context);
                },
                cancellationToken);
            }
        }

        /// <summary>
        /// Parse the content from a <c>TcpClient</c>-instance into an HTTP context object.
        /// </summary>
        /// <param name="listenerPrefix">The <see cref="ListenerPrefix" />, from which the <paramref name="client" /> was received.</param>
        /// <param name="client">The <see cref="TcpClient" />-instance to parse from.</param>
        /// <returns>The parsed <see cref="HttpContext" />-object.</returns>
        protected HttpContext WrapTcpClient(ListenerPrefix listenerPrefix, TcpClient client)
        {
            HttpContext ctx = new HttpContext(this, listenerPrefix, client);

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

            return ctx;
        }
    }
}