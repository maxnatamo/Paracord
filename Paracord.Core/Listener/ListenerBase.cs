using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Paracord.Shared.Models.Listener;

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
        public readonly X509Certificate2? Certificate;

        /// <summary>
        /// The list of prefixes the listener should listen on.
        /// </summary>
        public readonly ListenerPrefixCollection Prefixes;

        /// <summary>
        /// The list of listeners, one for each prefix in <see cref="Prefixes" />.
        /// </summary>
        protected readonly List<TcpListener> Listeners;

        /// <summary>
        /// Whether the listener is currently running.
        /// </summary>
        protected bool IsOpen { get; private set; } = false;

        /// <summary>
        /// Whether the listener has been disposed.
        /// </summary>
        protected bool Disposed { get; private set; } = false;

        /// <summary>
        /// Initialize a new <see cref="ListenerBase" />.
        /// </summary>
        /// <param name="certificate">An optional certificate to use for SSL connections. If null, SSL is disabled.</param>
        public ListenerBase(X509Certificate2? certificate = null)
        {
            this.InternalLock = new object();

            this.Certificate = certificate;
            this.Prefixes = new ListenerPrefixCollection();
            this.Listeners = new List<TcpListener>();

            Console.CancelKeyPress += (sender, e) =>
            {
                this.Stop();
            };
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
        /// <exception cref="ArgumentException">Thrown if a secure prefix was specified, but no certificate was added.</exception>
        public virtual void Start()
        {
            if(this.IsOpen)
            {
                return;
            }

            if(this.Prefixes.Any(v => v.Secure) && this.Certificate == null)
            {
                throw new ArgumentException("Prefix uses secure protocol, but no certificate was specified");
            }

            foreach(ListenerPrefix prefix in this.Prefixes)
            {
                IPAddress address = IPAddress.Parse(prefix.Address);
                int port = (int) prefix.Port;

                this.Listeners.Add(new TcpListener(address, port));
            }

            this.IsOpen = true;
            this.Listeners.ForEach(v => v.Start());
        }

        /// <summary>
        /// Stop accepting requests from the listener.
        /// </summary>
        /// <remarks>
        /// If the socket is already closed, nothing is done.
        /// </remarks>
        public virtual void Stop()
        {
            if(!this.IsOpen)
            {
                return;
            }

            this.IsOpen = false;
            this.Listeners.ForEach(v => v.Stop());
            this.Listeners.Clear();
        }
    }
}