using Paracord.Core.Http;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Extensions
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Redirect the accompanying <see cref="HttpRequest" /> to <paramref name="path" /> and set the status-code to <paramref name="statusCode" />.
        /// </summary>
        /// <remarks>
        /// The method sends the response.
        /// </remarks>
        /// <param name="response">The <see cref="HttpResponse" /> to redirect.</param>
        /// <param name="path">The new path to redirect to.</param>
        /// <param name="statusCode">The HTTP status code use.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref cref="statusCode" /> is not a 3xx status code.</exception>
        public static void Redirect(this HttpResponse response, string path, HttpStatusCode statusCode = HttpStatusCode.TemporaryRedirect)
        {
            if((int) statusCode > 399 || (int) statusCode < 300)
            {
                throw new ArgumentException("The status-code must use a 3xx status code.", nameof(statusCode));
            }

            response.Headers[HttpHeaders.Location] = path;
            response.StatusCode = statusCode;
            response.Send();
        }

        /// <inheritdoc cref="HttpResponseExtensions.Redirect(HttpResponse, string, HttpStatusCode)" />
        /// <param name="permanent">
        /// Whether the redirect is permanent or not.
        /// Specifies the status code (<see cref="HttpStatusCode.TemporaryRedirect" /> or <see cref="HttpStatusCode.PermanentRedirect" />).
        /// </param>
        public static void Redirect(this HttpResponse response, string path, bool permanent = false)
            => response.Redirect(path, permanent ? HttpStatusCode.PermanentRedirect : HttpStatusCode.TemporaryRedirect);
    }
}