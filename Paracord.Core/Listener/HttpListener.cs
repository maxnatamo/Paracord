using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

using Paracord.Shared.Http;
using Paracord.Shared.Models.Http;
using Paracord.Shared.Security.X509;

namespace Paracord.Core.Listener
{
    public class HttpListener : ListenerBase
    {
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
        /// Initialize a new <c>HttpListener</c>-instance with the specified IP-address and port.
        /// </summary>
        /// <param name="address">The IP-address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        /// <param name="sslCertificate">The SSL certificate to use for HTTPS connections. HTTPS is disabled, if null.</param>
        public HttpListener(string address, UInt16 port = 80, X509Certificate2? sslCertificate = null) : base(address, port)
        {
            this.SslCertificate = sslCertificate;
        }

        /// <summary>
        /// Accept a new HTTP request from the listener, synchronously.
        /// This method will block until a request is received.
        /// </summary>
        /// <returns>The received <c>HttpContext</c>-instance.</returns>
        public HttpContext AcceptRequest()
            => this.WrapTcpClient(this.AcceptClient());

        /// <summary>
        /// Accept a new HTTP request from the listener, asynchronously.
        /// This method will block until a request is received.
        /// </summary>
        /// <returns>A task, resolving to the received <c>HttpContext</c>-instance.</returns>
        public async Task<HttpContext> AcceptRequestAsync()
            => this.WrapTcpClient(await this.AcceptClientAsync());

        /// <summary>
        /// Parse the content from a <c>TcpClient</c>-instance into an HTTP context object.
        /// </summary>
        /// <param name="client">The <c>TcpClient</c>-instance to parse from.</param>
        /// <returns>The parsed <c>HttpContext</c>-object.</returns>
        protected HttpContext WrapTcpClient(TcpClient client)
        {
            Stream stream = client.GetStream();

            if(this.IsSecure)
            {
                stream = this.CreateSslStream(stream);
            }

            // Reading client.Available will reset it
            int bytesExpected = 0;
            int bytesRead = 0;

            // Wait until data is available
            while((bytesExpected = client.Available) == 0) { }

            Byte[] bytes = new Byte[bytesExpected];

            if((bytesRead = stream.Read(bytes, 0, bytes.Length)) != bytesExpected)
            {
                client.Close();
                Console.WriteLine(System.Text.Encoding.ASCII.GetString(bytes));
                throw new InvalidDataException($"TCP connection was ended abruptly; received {bytesRead} bytes, expected {bytesExpected} bytes.");
            }

            HttpContext ctx = new HttpContext(client);

            ctx.Request = new HttpRequest();
            ctx.Request.Context = ctx;

            HttpParser.DeserializeRequest(ctx.Request, bytes);

            ctx.Response = new HttpResponse();
            ctx.Response.Context = ctx;

            return ctx;
        }

        /// <summary>
        /// Wrap the specified stream with an <c>SslStream</c>-instance and authenticate as a server, using the <c>SslCertificate</c>.
        /// </summary>
        /// <param name="innerStream">The <c>Stream</c>-instance to wrap in an SSL stream.</param>
        /// <returns>The wrapped <c>SslStream</c>-instance.</returns>
        /// <exception cref="ArgumentNullException">The certificate, <c>SslCertificate</c>, is null.</exception>
        internal SslStream CreateSslStream(Stream innerStream)
        {
            ArgumentNullException.ThrowIfNull(this.SslCertificate, nameof(this.SslCertificate));

            lock(this.InternalLock)
            {
                SslStream sslStream = new SslStream(innerStream);
                sslStream.AuthenticateAsServer(serverCertificate: this.SslCertificate);

                return sslStream;
            }
        }
    }
}