using Paracord.Core.Compression;
using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware handles compression (gzip, deflate, etc.) on responses and requests.
    /// </summary>
    public class CompressionMiddleware : MiddlewareBase
    {
        public override void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            string[] acceptEncodingStrings = request.Headers[HttpHeaders.AcceptEncoding]?.Split(
                ',',
                StringSplitOptions.TrimEntries |
                StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };

            if(acceptEncodingStrings.Length == 0)
            {
                this.Next();
                return;
            }

            HttpQualityValue[] acceptEncodings = acceptEncodingStrings
                .Select(v => HttpQualityValue.Parse(v))
                .OrderByDescending(v => v.Weight)
                .ToArray();

            foreach(var acceptEncoding in acceptEncodings)
            {
                ICompressionProvider? provider = listener.CompressionProviders.FindProvider(acceptEncoding.Value);

                if(provider == null)
                {
                    continue;
                }

                byte[] compressedBody = provider.Compress(response.Body);

                response.Body.Seek(0, SeekOrigin.Begin);
                response.Body.Write(compressedBody);
                response.Body.Flush();

                response.Headers[HttpHeaders.Vary] = HttpHeaders.AcceptEncoding;
                response.Headers[HttpHeaders.ContentEncoding] = provider.AcceptedEncoding;
                response.Headers[HttpHeaders.ContentLength] = response.Body.Length.ToString();

                // While multiple compressions are available,
                // we don't take use of it.
                break;
            }

            this.Next();
        }
    }
}