using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Paracord.Shared.Models.Http
{
    /// <summary>
    /// Structure defining an HTTP-encoding, expressed by the <c>Accept-Encoding</c> and <c>Content-Encoding</c>.
    /// </summary>
    public class HttpQualityValueCollection : Collection<HttpQualityValue>
    {
        /// <summary>
        /// Parse a prioritized header value (e.g. <c>gzip;q=1.0, br;q=0.9</c>) to a <c>HttpQualityValue</c>-collection.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are not evaluated. The weights, if specified, are validated and parsed to floats.
        /// </para>
        /// <para>
        /// The weight is expected to be between 0.0 and 1.0, inclusive. If not, the method returns <c>false</c>.
        /// </para>
        /// </remarks>
        /// <param name="target">The HTTP header value to parse (e.g. <c>gzip;q=1.0, br;q=0.9</c>).</param>
        /// <param name="result">The resulting <c>HttpQualityValueCollection</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the string was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string target, [NotNullWhen(true)] out HttpQualityValueCollection? result)
        {
            result = new HttpQualityValueCollection();
            target = target.Trim();

            string[] elements = target.Split(",", StringSplitOptions.TrimEntries);

            if(!elements.Any())
            {
                result = null;
                return false;
            }

            foreach(string element in elements)
            {
                if(element.Length == 0)
                {
                    result = null;
                    return false;
                }

                if(!HttpQualityValue.TryParse(element, out var qualityValue))
                {
                    result = null;
                    return false;
                }

                result.Add(qualityValue);
            }

            return true;
        }

        /// <summary>
        /// Parse a string (e.g. <c>gzip;q=1.0, br;q=0.9</c>) to a native <c>HttpQualityValueCollection</c>-instance.
        /// </summary>
        /// <remarks>
        /// The values are not evaluated. The weight, if specified, is validated and parsed to a float.
        /// </remarks>
        /// <param name="target">The HTTP header value to parse (e.g. <c>gzip;q=1.0</c>).</param>
        /// <returns>The parsed <c>HttpQualityValueCollection</c>-instance.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        public static HttpQualityValueCollection Parse(string target)
        {
            if(HttpQualityValueCollection.TryParse(target, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied HTTP-encoding is improperly formatted.");
        }
    }
}