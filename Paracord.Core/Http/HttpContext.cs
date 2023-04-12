using System.Net;
using System.Net.Sockets;
using System.Text;

using Paracord.Core.Http;
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
        /// Create a new <see cref="HttpContext" />-instance with the <paramref name="client" />.
        /// </summary>
        /// <param name="listener">The <see cref="HttpListener" />-instance which handles this context.</param>
        /// <param name="client">The <see cref="TcpClient" />-instance to derive properties from.</param>
        public HttpContext(HttpListener listener, TcpClient client)
        {
            this.UniqueId = Guid.NewGuid().ToString();

            this.Listener = listener;
            this.Client = client;
            this.TTL = client.Client.Ttl;
            this.LocalEndpoint = (IPEndPoint?) client.Client.LocalEndPoint;
            this.RemoteEndpoint = (IPEndPoint?) client.Client.RemoteEndPoint;

            this.RequestStream = new HttpRequestStream(client.GetStream());
            this.ResponseStream = new HttpResponseStream(client.GetStream());
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

            this.Listener.ExecuteMiddleware(_ => _.BeforeResponseSent(this.Listener, this.Request, this.Response), true);

            byte[] responseBytes = HttpParser.SerializeResponse(this.Response);

            this.ResponseStream.Write(responseBytes);
            this.ResponseStream.Flush();

            this.Close();
        }
    }
}