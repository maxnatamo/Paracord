using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Paracord.Shared.Models.Http
{
    /// <summary>
    /// Structure defining a prioritized header value, such as <c>Accept-Encoding</c> and <c>Content-Encoding</c>.
    /// </summary>
    public class HttpQualityValue
    {
        /// <summary>
        /// The actual value of the field.
        /// </summary>
        public string Value { get; set; } = default!;

        /// <summary>
        /// The optional weight of the encoding. If not specified, defaults to 1.0.
        /// </summary>
        public float Weight { get; set; } = 1.0f;

        /// <summary>
        /// Parse a string (e.g. <c>gzip;q=1.0</c>) to a native <c>HttpQualityValue</c>-instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are not evaluated. The weight, if specified, is validated and parsed to a float.
        /// </para>
        /// <para>
        /// The input is trimmed to 256 characters, to avoid performance problems.
        /// </para>
        /// </remarks>
        /// <param name="target">The HTTP header value to parse (e.g. <c>gzip;q=1.0</c>).</param>
        /// <param name="result">The resulting <c>HttpQualityValue</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the string was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string target, [NotNullWhen(true)] out HttpQualityValue? result)
        {
            result = null;

            if(target == null)
            {
                return false;
            }

            if(target.Length > 256)
            {
                return false;
            }

            target = target.Trim();

            Regex pattern = new Regex(@"^(?<value>[a-zA-Z0-9_-]+)(;q=(?<qvalue>\d\.\d{1,3}))?$");
            Match match = pattern.Match(target);

            if(!match.Success)
            {
                return false;
            }

            result = new HttpQualityValue();
            result.Value = match.Groups["value"].Value;

            if(match.Groups.ContainsKey("qvalue") && match.Groups["qvalue"].Length > 0)
            {
                if(!float.TryParse(match.Groups["qvalue"].Value, NumberStyles.AllowDecimalPoint, null, out var weight))
                {
                    result = null;
                    return false;
                }

                if(weight > 1.0f || weight < 0.0f)
                {
                    result = null;
                    return false;
                }

                result.Weight = weight;
            }

            return true;
        }

        /// <summary>
        /// Parse a string (e.g. <c>gzip;q=1.0</c>) to a native <c>HttpQualityValue</c>-instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are not evaluated. The weight, if specified, is validated and parsed to a float.
        /// </para>
        /// <para>
        /// The input is trimmed to 256 characters, to avoid performance problems.
        /// </para>
        /// </remarks>
        /// <param name="target">The HTTP header value to parse (e.g. <c>gzip;q=1.0</c>).</param>
        /// <returns>The parsed <c>HttpQualityValue</c>-instance.</returns>
        /// <exception cref="FormatException">Thrown if the parsing failed.</exception>
        public static HttpQualityValue Parse(string target)
        {
            if(HttpQualityValue.TryParse(target, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied HTTP header is improperly formatted.");
        }
    }
}