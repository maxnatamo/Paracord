using System.Text;

using Paracord.Core.Http;
using Paracord.Shared.Models.Http;
using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Core.Parsing.Http
{
    public static partial class HttpParser
    {
        /// <summary>
        /// Parse the specified Byte-array into an HTTP request.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest" /> to fill.</param>
        /// <param name="stream">The Byte-array to parse.</param>
        /// <exception cref="FormatException">Thrown if the message doesn't contain the content-delimiter string.</exception>
        /// <exception cref="NotImplementedException">Thrown if the status-line is missing or improperly formatted.</exception>
        /// <exception cref="FormatException">Thrown if the status-line has more or less and 3 separate components.</exception>
        /// <exception cref="NotImplementedException">Thrown if the defined HTTP verb is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request" /> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="HttpRequest.Headers" /> is null.</exception>
        /// <exception cref="FormatException">Thrown if a header contains less than 2 colons.</exception>
        /// <exception cref="NotImplementedException">Thrown if the <c>Content-Length</c> header doesn't match the body length.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request" /> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <see cref="HttpRequest.Headers" /> is null.</exception>
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
        /// <exception cref="NotImplementedException">Thrown if the <c>Content-Length</c> header doesn't match the body length.</exception>
        internal static void DeserializeBody(HttpRequest request, byte[] bodyData)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            ArgumentNullException.ThrowIfNull(request.Body, nameof(request.Body));

            string? contentLengthHeader = request.Headers[HttpHeaders.ContentLength];

            if(contentLengthHeader != null && bodyData.Length.ToString() != contentLengthHeader)
            {
                throw new NotImplementedException(
                    $"Content-Length does not match body content;" +
                    $"{request.ContentLength} != {contentLengthHeader}"
                );
            }

            request.Body.Write(bodyData);
        }
    }
}