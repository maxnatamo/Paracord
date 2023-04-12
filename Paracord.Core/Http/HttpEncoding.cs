using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Paracord.Core.Http
{
    /// <summary>
    /// Structure defining an HTTP-encoding, expressed by the <c>Accept-Encoding</c> and <c>Content-Encoding</c>.
    /// </summary>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Accept-Encoding">Reference for Accept-Encoding</seealso>
    /// <seealso href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Encoding">Reference for Content-Encoding</seealso>
    public class HttpEncoding
    {
        /// <summary>
        /// The encoding and/or identifier for the encoding.
        /// </summary>
        public string Encoding { get; set; } = default!;

        /// <summary>
        /// The optional weight of the encoding. Only used for <c>Accept-Encoding</c> headers.
        /// </summary>
        public float Weight { get; set; } = 1.0f;

        /// <summary>
        /// Parse a string (e.g. gzip;q=1.0) to a native <c>HttpEncoding</c>-instance.
        /// </summary>
        /// <remarks>
        /// The values of the encoding are not evaluated.
        /// </remarks>
        /// <param name="target">The HTTP encoding header value to parse (e.g. gzip;q=1.0).</param>
        /// <param name="result">The resulting <c>HttpEncoding</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the encoding was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string target, [NotNullWhen(true)] out HttpEncoding? result)
        {
            result = null;
            target = target.Trim();

            Regex pattern = new Regex(@"^(?<encoding>[a-zA-Z0-9_-]+)(;q=(?<qvalue>\d\.\d{1,3}))?$");
            Match match = pattern.Match(target);

            if(!match.Success)
            {
                return false;
            }

            result = new HttpEncoding();
            result.Encoding = match.Groups["encoding"].Value;

            if(match.Groups.ContainsKey("qvalue") && match.Groups["qvalue"].Length > 0)
            {
                if(!float.TryParse(match.Groups["qvalue"].Value, NumberStyles.AllowDecimalPoint, null, out var weight))
                {
                    result = null;
                    return false;
                }

                result.Weight = weight;
            }

            return true;
        }

        /// <summary>
        /// Parse a string (e.g. gzip;q=1.0) to a native <c>HttpEncoding</c>-instance.
        /// </summary>
        /// <remarks>
        /// The values of the encoding are not evaluated.
        /// </remarks>
        /// <param name="target">The HTTP encoding to parse (e.g. gzip;q=1.0).</param>
        /// <returns>The parsed <c>HttpEncoding</c>-instance.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        public static HttpEncoding Parse(string target)
        {
            if(HttpEncoding.TryParse(target, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied HTTP-encoding is improperly formatted.");
        }
    }
}