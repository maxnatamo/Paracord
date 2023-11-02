using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware handles the "Server"-header on the HTTP response.
    /// </summary>
    public class ServerMiddleware : MiddlewareBase
    {
        public override void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            response.Headers[HttpHeaders.Server] = "Paracord";
            this.Next();
        }
    }
}