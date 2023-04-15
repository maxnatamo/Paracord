using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;

using Paracord.Core.Http;
using Paracord.Shared.Models.Listener;
using HttpListener = Paracord.Core.Listener.HttpListener;

namespace Paracord.Shared.Models.Http
{
    /// <summary>
    /// Context for an HTTP connection, made over TCP.
    /// </summary>
    public class HttpContext
    {
        /// <summary>
        /// The underlying <see cref="TcpClient" />-instance for handling the corresponding TCP socket.
        /// </summary>
        public readonly TcpClient Client;

        /// <summary>
        /// Whether the connection is currently open.
        /// </summary>
        public bool IsOpen
        {
            get => this.Client.Connected;
        }

        /// <summary>
        /// The listener which handles this context.
        /// </summary>
        public readonly HttpListener Listener;

        /// <summary>
        /// The listener prefix, from which the context is operating on.
        /// </summary>
        public readonly ListenerPrefix ListenerPrefix;

        /// <summary>
        /// The local <see cref="IPEndPoint" />-instance for the current client, if any. Otherwise, null.
        /// </summary>
        public IPEndPoint? LocalEndpoint { get; } = default!;

        /// <summary>
        /// The <see cref="HttpRequest" />-instance for the current context.
        /// </summary>
        public HttpRequest Request { get; set; } = default!;

        /// <summary>
        /// The <see cref="HttpRequestStream" />-instance for the current TCP-connection.
        /// </summary>
        protected HttpRequestStream RequestStream { get; }

        /// <summary>
        /// The <see cref="HttpResponse" />-instance for the current context.
        /// </summary>
        public HttpResponse Response { get; set; } = default!;

        /// <summary>
        /// The <see cref="HttpResponseStream" />-instance for the current TCP-connection.
        /// </summary>
        protected HttpResponseStream ResponseStream { get; }

        /// <summary>
        /// The remote <see cref="IPEndPoint" />-instance for the current client, if any. Otherwise, null.
        /// </summary>
        public IPEndPoint? RemoteEndpoint { get; } = default!;

        /// <summary>
        /// Time To Live (TTL) value from IP-packets.
        /// </summary>
        public short TTL { get; set; } = default!;

        /// <summary>
        /// The unique identifier for the connection.
        /// </summary>
        public string UniqueId { get; set; } = default!;

        /// <summary>
        /// The unencrypted, plain-text network stream for the TCP connection.
        /// </summary>
        protected readonly NetworkStream PlainStream;

        /// <summary>
        /// The encrypted, SSL network stream for the TCP connection.
        /// </summary>
        protected readonly SslStream? EncryptedStream = null;

        /// <summary>
        /// The main network stream for the TCP connection.
        /// 
        /// <para>
        /// If TLS is enabled, it resolves to the SSL stream, <see cref="EncryptedStream" />.
        /// Otherwise, it resolves to the plain-text stream, <see cref="PlainStream" />.
        /// </para>
        /// </summary>
        protected internal Stream ConnectionStream
        {
            get => this.EncryptedStream != null ? this.EncryptedStream : this.PlainStream;
        }

        /// <summary>
        /// Create a new <see cref="HttpContext" />-instance with the <paramref name="client" />.
        /// </summary>
        /// <param name="listener">The <see cref="HttpListener" />-instance which handles this context.</param>
        /// <param name="listenerPrefix">The <see cref="ListenerPrefix" />, from which the context is operating on.</param>
        /// <param name="client">The <see cref="TcpClient" />-instance to derive properties from.</param>
        public HttpContext(HttpListener listener, ListenerPrefix listenerPrefix, TcpClient client)
        {
            this.UniqueId = Guid.NewGuid().ToString();
            this.ListenerPrefix = listenerPrefix;

            this.Listener = listener;
            this.Client = client;
            this.TTL = client.Client.Ttl;
            this.LocalEndpoint = (IPEndPoint?) client.Client.LocalEndPoint;
            this.RemoteEndpoint = (IPEndPoint?) client.Client.RemoteEndPoint;

            this.PlainStream = client.GetStream();

            if(this.ListenerPrefix.Secure && this.Listener.Certificate != null)
            {
                this.EncryptedStream = new SslStream(this.PlainStream, true);
                this.EncryptedStream.AuthenticateAsServer(this.Listener.Certificate, false, SslProtocols.Tls13 | SslProtocols.Tls12, false);
            }

            this.RequestStream = new HttpRequestStream(this.ConnectionStream);
            this.ResponseStream = new HttpResponseStream(this.ConnectionStream);
        }

        /// <summary>
        /// Close the TCP connection.
        /// </summary>
        public void Close()
            => this.Client.Close();

        /// <summary>
        /// Send the response to the receiving party.
        /// </summary>
        /// <remarks>
        /// The TCP-connection is not closed, to allow for multiplexing.
        /// </remarks>
        /// <exception cref="InvalidDataException">Thrown if the <c>Content-Length</c> header is set and doesn't match the body length.</exception>
        public void Send()
        {
            if(!this.IsOpen)
            {
                throw new ObjectDisposedException("HttpContext", "The socket has been closed.");
            }

            this.Listener.ExecuteMiddleware(_ => _.BeforeResponseSent(this.Listener, this.Request, this.Response), true);

            // Default to the length of the body.
            if(this.Response.Headers["content-length"] == null)
            {
                this.Response.Headers["content-length"] = this.Response.ContentLength.ToString();
            }

            // Throw if they're different
            if(this.Response.Headers["content-length"] != this.Response.ContentLength.ToString())
            {
                throw new InvalidDataException("Content-Length parameter doesn't match the length of the Body property on the HttpResponse.");
            }

            byte[] responseBytes = HttpParser.SerializeResponse(this.Response);

            this.ResponseStream.Write(responseBytes);
            this.ResponseStream.Flush();

            this.Close();
        }
    }
}