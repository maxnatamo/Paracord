using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Paracord.Core.Listener
{
    public abstract class ListenerBase : IDisposable
    {
        /// <summary>
        /// Lock for internal threaded operations.
        /// </summary>
        internal readonly object InternalLock;

        /// <summary>
        /// An optional certificate to use for SSL connections. If null, SSL is disabled.
        /// </summary>
        protected readonly X509Certificate2? Certificate;

        /// <summary>
        /// The actual TCP listener.
        /// </summary>
        protected TcpListener Listener { get; set; }

        /// <summary>
        /// The address which the listener should listen on.
        /// </summary>
        public IPAddress ListenAddress { get; protected set; }

        /// <summary>
        /// The port number which the listener should listen on.
        /// </summary>
        public UInt16 ListenPort { get; protected set; }

        /// <summary>
        /// Whether the listener is currently running.
        /// </summary>
        protected bool IsOpen { get; private set; } = false;

        /// <summary>
        /// Whether the listener has been disposed.
        /// </summary>
        protected bool Disposed { get; private set; } = false;

        /// <summary>
        /// Initialize a new ListenerBase, with the specified listener-address and -port.
        /// </summary>
        /// <param name="address">The IP-address to listen on.</param>
        /// <param name="port">The port number to listen on.</param>
        /// <param name="certificate">An optional certificate to use for SSL connections. If null, SSL is disabled.</param>
        public ListenerBase(string address, UInt16 port, X509Certificate2? certificate = null)
        {
            this.InternalLock = new object();

            this.ListenAddress = address switch
            {
                var a when a == "localhost"        => IPAddress.Loopback,
                var a when string.IsNullOrEmpty(a) => IPAddress.Any,
                var a when a == "*"                => IPAddress.Any,

                _ => IPAddress.Parse(address)
            };

            this.Certificate = certificate;

            this.ListenPort = port;
            this.Listener = new TcpListener(this.ListenAddress, this.ListenPort);
        }

        /// <summary>
        /// Dispose the listener and free up any allocated resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            ObjectDisposedException.ThrowIf(this.Disposed, this);

            this.Stop();
            this.Disposed = true;
        }

        /// <summary>
        /// Start accepting requests from the listener.
        /// </summary>
        /// <remarks>
        /// If the socket is already open, nothing is done.
        /// </remarks>
        public void Start()
        {
            if(this.IsOpen)
            {
                return;
            }

            this.IsOpen = true;
            this.Listener.Start();
        }

        /// <summary>
        /// Stop accepting requests from the listener.
        /// </summary>
        /// <remarks>
        /// If the socket is already closed, nothing is done.
        /// </remarks>
        public void Stop()
        {
            if(!this.IsOpen)
            {
                return;
            }

            this.IsOpen = false;
            this.Listener.Stop();
        }

        /// <summary>
        /// Accept a new client from the listener, synchronously.
        /// This method will block until a client is available.
        /// </summary>
        /// <returns>The accepted <c>TcpClient</c>-instance.</returns>
        protected TcpClient AcceptClient()
            => this.Listener.AcceptTcpClient();

        /// <summary>
        /// Accept a new client from the listener, asynchronously.
        /// This method will block until a client is available.
        /// </summary>
        /// <returns>A task, resolving to the accepted <c>TcpClient</c>-instance.</returns>
        protected async Task<TcpClient> AcceptClientAsync()
            => await this.Listener.AcceptTcpClientAsync();

        /// <summary>
        /// Wrap the specified stream with an <c>SslStream</c>-instance and authenticate as a server, using the <c>SslCertificate</c>.
        /// </summary>
        /// <param name="innerStream">The <c>Stream</c>-instance to wrap in an SSL stream.</param>
        /// <returns>The wrapped <c>SslStream</c>-instance.</returns>
        /// <exception cref="ArgumentNullException">The certificate, <c>SslCertificate</c>, is null.</exception>
        protected SslStream CreateSslStream(Stream innerStream)
        {
            ArgumentNullException.ThrowIfNull(this.Certificate, nameof(this.Certificate));

            lock(this.InternalLock)
            {
                SslStream sslStream = new SslStream(innerStream, true);
                sslStream.AuthenticateAsServer(this.Certificate, false, SslProtocols.Tls13 | SslProtocols.Tls12, false);

                return sslStream;
            }
        }
    }
}