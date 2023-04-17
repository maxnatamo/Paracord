using System.Text;
using Paracord.Core.Http;

namespace Paracord.Core.Parsing.Http
{
    public static partial class HttpParser
    {
        /// <summary>
        /// The current version of HTTP supported by Paracord.
        /// /// </summary>
        public static HttpVersion CurrentHttpVersion { get; } = new HttpVersion(1, 1);

        /// <summary>
        /// Byte sequence indicating the end-of-line, according to the HTTP/1.1 specification.
        /// </summary>
        /// <seealso href="https://datatracker.ietf.org/doc/html/rfc9112#name-message-parsing">Reference</seealso>
        private static readonly byte[] EndOfLine = new byte[] { 0x0D, 0x0A };

        /// <summary>
        /// Byte sequence indicating the end-of-line, according to the HTTP/1.1 specification, in string format.
        /// </summary>
        /// <seealso href="https://datatracker.ietf.org/doc/html/rfc9112#name-message-parsing">Reference</seealso>
        private static readonly string EndOfLineString = Encoding.ASCII.GetString(EndOfLine);

        /// <summary>
        /// Byte sequence indicating the end of the HTTP header and start of body.
        /// </summary>
        private static readonly byte[] ContentDelimiter = new byte[] { 0x0D, 0x0A, 0x0D, 0x0A };

        /// <summary>
        /// Byte sequence indicating the end of the HTTP header and start of body, in string format.
        /// </summary>
        private static readonly string ContentDelimiterString = Encoding.ASCII.GetString(ContentDelimiter);
    }
}