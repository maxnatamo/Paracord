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
        protected readonly X509Certificate2 Certificate;

        /// <summary>
        /// Initialize a new <c>HttpListener</c>-instance with the specified IP-address and port.
        /// </summary>
        /// <param name="address">The IP-address to listen on.</param>
        /// <param name="port">The port to listen on.</param>
        public HttpListener(string address, UInt16 port = 80) : base(address, port)
        {
            this.Certificate = new X509CertificateBuilder()
                .SetCommonName("localhost")
                .SetCountry("DK")
                .Build();
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

        internal SslStream CreateSslStream(Stream innerStream)
        {
            lock(this.InternalLock)
            {
                SslStream sslStream = new SslStream(innerStream);
                sslStream.AuthenticateAsServer(
                    serverCertificate: this.Certificate);

                return sslStream;
            }
        }
    }
}