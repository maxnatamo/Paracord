using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Paracord.Core.Http
{
    /// <summary>
    /// Structure defining an HTTP-version, using the <major>.<minor> numbering scheme.
    /// </summary>
    public class HttpVersion
    {
        /// <summary>
        /// The major field of the HTTP version.
        /// </summary>
        public byte Major { get; set; } = 0;

        /// <summary>
        /// The minor field of the HTTP version.
        /// </summary>
        public byte Minor { get; set; } = 0;

        /// <summary>
        /// Initialize a new HttpVersion-instance with the specified major-, minor- digits.
        /// </summary>
        /// <param name="major">The major-digit of the schema.</param>
        /// <param name="minor">The major-digit of the schema.</param>
        public HttpVersion(byte major, byte minor = 0)
        {
            this.Major = major;
            this.Minor = minor;
        }

        /// <summary>
        /// Override implicit conversion to string.
        /// </summary>
        public override string ToString()
            => $"HTTP/{this.Major}.{this.Minor}";

        /// <summary>
        /// Parse a string (e.g. HTTP/1.1) to a native <c>HttpVersion</c>-instance.
        /// </summary>
        /// <param name="httpVersion">The HTTP version string to parse (e.g. HTTP/1.1).</param>
        /// <param name="result">The resulting <c>HttpVersion</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the version was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string httpVersion, [NotNullWhen(true)] out HttpVersion? result)
        {
            result = null;

            Regex pattern = new Regex(@"^HTTP/(?<major>\d)\.(?<minor>\d)$");
            Match match = pattern.Match(httpVersion);

            if(!match.Success)
            {
                return false;
            }

            result = new HttpVersion(
                byte.Parse(match.Groups["major"].Value),
                byte.Parse(match.Groups["minor"].Value)
            );
            return true;
        }

        /// <summary>
        /// Parse a string (e.g. HTTP/1.1) to a native <c>HttpVersion</c>-instance.
        /// </summary>
        /// <param name="httpVersion">The HTTP version string to parse (e.g. HTTP/1.1).</param>
        /// <returns>The parsed <c>HttpVersion</c>-instance.</returns>
        public static HttpVersion Parse(string httpVersion)
        {
            if(HttpVersion.TryParse(httpVersion, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied HTTP-version is improperly formatted.");
        }
    }
}