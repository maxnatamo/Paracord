using System.Text;

using Paracord.Shared.Models.Http;
using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Shared.Http
{
    public static class HttpParser
    {
        /// <summary>
        /// The current version of HTTP supported by Paracord.
        /// </summary>
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

        /// <summary>
        /// Parse the specified Byte-array into an HTTP request.
        /// </summary>
        /// <param name="request">The <c>HttpRequest</c> to fill.</param>
        /// <param name="stream">The Byte-array to parse.</param>
        /// <exception cref="FormatException">Thrown if the message doesn't contain the content-delimiter string.</exception>
        /// <exception cref="NotImplementedException">Thrown if the status-line is missing or improperly formatted.</exception>
        /// <exception cref="FormatException">Thrown if the status-line has more or less and 3 separate components.</exception>
        /// <exception cref="NotImplementedException">Thrown if the defined HTTP verb is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if request.Headers is null.</exception>
        /// <exception cref="FormatException">Thrown if a header contains less than 2 colons.</exception>
        /// <exception cref="NotImplementedException">Thrown if the Content-Length header doesn't match the body length.</exception>
        public static void DeserializeRequest(HttpRequest request, byte[] stream)
        {
            string[] requestData = Encoding.UTF8.GetString(stream).Split(ContentDelimiterString);

            if(requestData.Length != 2)
            {
                throw new FormatException("HTTP message was improperly formatted");
            }

            // Separate headers and body
            string[] headerData = requestData[0].Split(EndOfLineString);
            byte[] bodyData = Encoding.UTF8.GetBytes(requestData[1]);

            HttpParser.DeserializeStatusLine(request, headerData);
            HttpParser.DeserializeHeaders(request, headerData);
            HttpParser.DeserializeBody(request, bodyData);
        }

        /// <summary>
        /// Parse the status-line of an HTTP request and return the request by reference.
        /// </summary>
        /// <param name="request">The request to parse. Parsed by reference.</param>
        /// <param name="headerData">String-array of all lines in the HTTP message.</param>
        /// <exception cref="NotImplementedException">Thrown if the status-line is missing or improperly formatted.</exception>
        /// <exception cref="FormatException">Thrown if the status-line has more or less and 3 separate components.</exception>
        /// <exception cref="NotImplementedException">Thrown if the defined HTTP verb is invalid.</exception>
        internal static void DeserializeStatusLine(HttpRequest request, string[] headerData)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            if(headerData.Length == 0)
            {
                throw new NotImplementedException("Status-line not included or improperly formatted.");
            }

            string statusLine = headerData[0];

            // Split according to HTTP/1.1.
            string[] statusComponents = statusLine.Split(' ');

            if(statusComponents.Length != 3)
            {
                throw new FormatException("Invalid status-line formatting.");
            }

            try
            {
                // Parse request method
                request.Method = (HttpMethod) Enum.Parse(typeof(HttpMethod), statusComponents[0]);
            }
            catch(Exception e)
            {
                throw new NotImplementedException("Invalid HTTP verb defined", e);
            }

            request.Target = HttpTarget.Parse(statusComponents[1]);
            request.Protocol = HttpVersion.Parse(statusComponents[2]);
        }

        /// <summary>
        /// Parse the headers of an HTTP request and return the request by reference.
        /// </summary>
        /// <param name="request">The request to parse. Parsed by reference.</param>
        /// <param name="headerData">String-array of all lines in the HTTP message.</param>
        /// <exception cref="ArgumentNullException">Thrown if request is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if request.Headers is null.</exception>
        /// <exception cref="FormatException">Thrown if a header contains less than 2 colons.</exception>
        internal static void DeserializeHeaders(HttpRequest request, string[] headerData)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Headers, nameof(request.Headers));

            // Ignore, if no headers are attached.
            if(headerData.Length <= 1)
            {
                return;
            }

            for(int i = 1; i < headerData.Length; i++)
            {
                string headerLine = headerData[i];
                string[] headerSegments = headerLine.Split(':', 2);

                if(headerSegments.Length != 2)
                {
                    throw new FormatException("Header was improperly formatted.");
                }

                string headerName = headerSegments[0].Trim();
                string headerValue = headerSegments[1].Trim();

                request.Headers.Add(headerName.ToLower(), headerValue);
            }
        }

        /// <summary>
        /// Parse the body of an HTTP request and return the request by reference.
        /// </summary>
        /// <remarks>
        /// This method depends on properly-parsed headers.
        /// The headers should be parsed before this method is called.
        /// </remarks>
        /// <param name="request">The request to parse. Parsed by reference.</param>
        /// <param name="bodyData">Byte-array, representing the body content.</param>
        /// <exception cref="NotImplementedException">Thrown if the Content-Length header doesn't match the body length.</exception>
        internal static void DeserializeBody(HttpRequest request, byte[] bodyData)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Body, nameof(request.Body));

            string? contentLengthHeader = request.Headers["content-length"];

            if(contentLengthHeader != null && bodyData.Length.ToString() != contentLengthHeader)
            {
                throw new NotImplementedException(
                    $"Content-Length does not match body content;" +
                    $"{request.ContentLength} != {contentLengthHeader}"
                );
            }

            request.Body.Write(bodyData);
        }

        /// <summary>
        /// Parse the specified <c>HttpResponse</c> into a valid HTTP response.
        /// </summary>
        /// <param name="response">The response to serialize.</param>
        /// <returns>A valid HTTP response.</returns>
        public static string SerializeResponse(HttpResponse response)
        {
            string content = "";

            // Append status-line
            content += $"{CurrentHttpVersion.ToString()} ";
            content += $"{((int) response.StatusCode)}";

            // Append headers
            foreach(var key in response.Headers.AllKeys)
            {
                content += HttpParser.EndOfLineString + $"{key}: {response.Headers[key]}";
            }

            // Append content-delimiter
            content += HttpParser.ContentDelimiterString;

            // Append body, if present
            if(response.Body.Length > 0)
            {
                byte[] buffer = new byte[response.Body.Length];

                response.Body.Seek(0, SeekOrigin.Begin);
                response.Body.Read(buffer);

                content += Encoding.ASCII.GetString(buffer);
            }

            return content;
        }
    }
}