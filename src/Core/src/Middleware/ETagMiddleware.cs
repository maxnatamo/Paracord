using System.Security.Cryptography;
using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Shared.Extensions;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware adds the "ETag"-header to eligible responses, to aid in caching.
    /// </summary>
    public class ETagMiddleware : MiddlewareBase
    {
        /// <summary>
        /// The maximum response size, for the response to be eligible for the "ETag"-header.
        /// </summary>
        /// <remarks>
        /// Expands to 32KB (<c>= 32 * 1024</c>)
        /// </remarks>
        public readonly int MaximumResponseSize = 32 * 1024;

        public override void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            this.Next();

            if(!this.IsEligible(response))
            {
                return;
            }

            string digest = this.ComputeDigest(response);

            response.Headers[HttpHeaders.ETag] = digest;

            if(!this.IsMatch(request, digest))
            {
                response.StatusCode = HttpStatusCode.PreconditionFailed;
                response.Body.SetLength(0);
                return;
            }
        }

        /// <summary>
        /// Whether the specified response is eligible for the "ETag"-header.
        /// </summary>
        /// <param name="response">The <see cref="HttpResponse" />-instance to check for.</param>
        /// <returns>True, if the response is eligible. Otherwise, false.</returns>
        protected bool IsEligible(HttpResponse response)
        {
            if(!response.IsSuccessful)
            {
                return false;
            }

            if(response.Headers.Get(HttpHeaders.ETag) != null)
            {
                return false;
            }

            if(response.ContentLength > MaximumResponseSize)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Compute the hash digest from the body of the response.
        /// </summary>
        /// <remarks>
        /// The hash is generated with SHA256.
        /// </remarks>
        /// <param name="response">The response to compute the digest from.</param>
        /// <returns>The digest of the body, wrapped in double-quotes.</returns>
        protected string ComputeDigest(HttpResponse response)
        {
            using SHA256 sha256 = SHA256.Create();

            response.Body.Seek(0, SeekOrigin.Begin);
            byte[] digestBytes = sha256.ComputeHash(response.Body);

            response.Body.Seek(0, SeekOrigin.Begin);
            string digest = Convert.ToBase64String(digestBytes);

            return $"\"{digest}\"";
        }

        /// <summary>
        /// Whether the specified request matches the computed digest value.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest" />-instance to check for.</param>
        /// <param name="digest">The generated digest to match with.</param>
        /// <returns>True, if the ETag is matched. Otherwise, false.</returns>
        protected bool IsMatch(HttpRequest request, string digest)
        {
            string? ifMatch = request.Headers[HttpHeaders.IfMatch];
            string? ifNoneMatch = request.Headers[HttpHeaders.IfNoneMatch];

            if(ifMatch != null && !this.IsHeaderMatched(ifMatch, digest))
            {
                return false;
            }

            if(ifNoneMatch != null && this.IsHeaderMatched(ifNoneMatch, digest))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Whether a single header contains the specified digest.
        /// </summary>
        /// <remarks>
        /// If the header only contains '*', it will return <c>true</c>.
        /// </remarks>
        /// <param name="headerValue">The full header value of the header.</param>
        /// <param name="digest">The digest to check for.</param>
        /// <returns>True, if the header matches the digest. Otherwise, false.</returns>
        protected bool IsHeaderMatched(string headerValue, string digest)
        {
            StringSplitOptions opts = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
            List<string> digests = headerValue.Split(',', opts).ToList();

            // Always return true for 'any' resource
            if(digests.Count == 1 && digests[0] == "*")
            {
                return true;
            }

            // Remove 'W/', if present
            digests.Select(v =>
            {
                if(v.StartsWith("W/"))
                {
                    v = v.Substring(2);
                }
                return v;
            });

            return digests.Any(v => v == digest);
        }
    }
}