using Paracord.Core.Http;

namespace Paracord.Core.Listener
{
    public abstract class MiddlewareBase
    {
        /// <summary>
        /// Whether to continue to the next middleware.
        /// </summary>
        internal bool UseNextMiddleware { get; private set; } = false;

        /// <summary>
        /// Executed when the server has been started and is ready to recieve requests.
        /// </summary>
        /// <param name="listener">The listener which the middleware is attached to.</param>
        public virtual void OnServerStarted(HttpListener listener)
            => this.Next();

        /// <summary>
        /// Executed before the server is set to close.
        /// </summary>
        /// <param name="listener">The listener which the middleware is attached to.</param>
        public virtual void OnServerClosed(HttpListener listener)
            => this.Next();

        /// <summary>
        /// Executed after the request has been recieved and parsed, but before the response is sent.
        /// </summary>
        /// <param name="listener">The listener which the middleware is attached to.</param>
        /// <param name="request">The request that was recieved by the listener.</param>
        /// <param name="response">The response to be sent by the listener.</param>
        public virtual void AfterRequestReceived(HttpListener listener, HttpRequest request, HttpResponse response)
            => this.Next();

        /// <summary>
        /// Executed before the response is sent.
        /// This method is executed after the (optional) response body is written and content-length is set.
        /// </summary>
        /// <param name="listener">The listener which the middleware is attached to.</param>
        /// <param name="request">The request that was recieved by the listener.</param>
        /// <param name="response">The response to be sent by the listener.</param>
        public virtual void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
            => this.Next();

        /// <summary>
        /// Proceed to the next middleware.
        /// </summary>
        protected void Next()
            => this.UseNextMiddleware = true;
    }
}