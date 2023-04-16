using Paracord.Core.Compression;
using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// Options for <see cref="CompressionMiddleware" />.
    /// </summary>
    public class CompressionMiddlewareOptions
    {
        /// <summary>
        /// Available providers for <see cref="CompressionMiddleware" />.
        /// </summary>
        public readonly CompressionProviderCollection Providers = new CompressionProviderCollection
        {
            new BrotliCompressionProvider(),
            new DeflateCompressionProvider(),
            new GzipCompressionProvider(),
        };
    }

    /// <summary>
    /// This middleware handles compression (gzip, deflate, etc.) on responses and requests.
    /// </summary>
    public class CompressionMiddleware : MiddlewareBase
    {
        /// <summary>
        /// Options for <see cref="CompressionMiddleware" />.
        /// </summary>
        public readonly CompressionMiddlewareOptions Options = new CompressionMiddlewareOptions();

        /// <summary>
        /// Initialize a new <see cref="CompressionMiddleware" />, with the specified <see cref="CompressionMiddlewareOptions" />.
        /// </summary>
        public CompressionMiddleware()
        {

        }

        /// <summary>
        /// Initialize a new <see cref="CompressionMiddleware" />, with the specified <see cref="CompressionMiddlewareOptions" />.
        /// </summary>
        /// <param name="options">The options for the middleware.</param>
        public CompressionMiddleware(CompressionMiddlewareOptions options)
        {
            this.Options = options;
        }

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
                ICompressionProvider? provider = this.Options.Providers.FindProvider(acceptEncoding.Value);

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