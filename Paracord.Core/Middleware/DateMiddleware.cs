using Paracord.Core.Http;
using Paracord.Core.Listener;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware handles the "Date"-header on the HTTP response.
    /// </summary>
    public class DateMiddleware : MiddlewareBase
    {
        public override void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            response.Headers["Date"] = DateTime.Now.ToString("R");
            this.Next();
        }
    }
}