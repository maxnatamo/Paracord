using Paracord.Core.Http;
using Paracord.Core.Listener;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware handles the "Server"-header on the HTTP response.
    /// </summary>
    public class ServerMiddleware : MiddlewareBase
    {
        public override void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            response.Headers["Server"] = "Paracord";
            this.Next();
        }
    }
}