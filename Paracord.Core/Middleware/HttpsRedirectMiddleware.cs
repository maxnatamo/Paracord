using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware handles automatic redirection from HTTP to HTTPS.
    /// </summary>
    public class HttpsRedirectMiddleware : MiddlewareBase
    {
        public override void AfterRequestReceived(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            bool hasSecurePrefix = listener.Prefixes.Any(v => v.Secure);
            bool isRequestSecure = request.Context.ListenerPrefix.Secure;

            if(!hasSecurePrefix)
            {
                this.Next();
                return;
            }

            if(isRequestSecure)
            {
                this.Next();
                return;
            }

            // If the client explicitly refuses to upgrade, skip.
            if(request.Headers[HttpHeaders.UpgradeInsecureRequests] == "0")
            {
                this.Next();
                return;
            }

            if(request.Headers[HttpHeaders.UpgradeInsecureRequests] == "1")
            {
                response.Headers[HttpHeaders.Vary] += HttpHeaders.UpgradeInsecureRequests;
            }

            response.StatusCode = HttpStatusCode.MovedPermanently;
            response.Headers[HttpHeaders.Location] = listener.Prefixes.First(v => v.Secure).ToString();
            response.Send();
        }
    }
}