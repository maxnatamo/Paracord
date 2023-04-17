using System.Text;

using Paracord.Core.Http;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Parsing.Http
{
    public static partial class HttpParser
    {
        /// <summary>
        /// Parse the specified <see cref="HttpResponse" /> into a valid HTTP response.
        /// </summary>
        /// <param name="response">The response to serialize.</param>
        /// <returns>A valid HTTP response.</returns>
        public static byte[] SerializeResponse(HttpResponse response)
        {
            using MemoryStream content = new MemoryStream();

            Action<string> WriteStringContent = (value) =>
            {
                content.Write(Encoding.ASCII.GetBytes(value));
            };

            // Append status-line
            WriteStringContent($"{CurrentHttpVersion.ToString()} {((int) response.StatusCode)}");

            // Append headers
            foreach(var key in response.Headers.AllKeys)
            {
                WriteStringContent(HttpParser.EndOfLineString + $"{key}: {response.Headers[key]}");
            }

            // Append cookies
            foreach(var key in response.Cookies.AllKeys)
            {
                WriteStringContent(HttpParser.EndOfLineString + $"{HttpHeaders.SetCookie}: {key}={response.Headers[key]}");
            }

            // Append content-delimiter
            WriteStringContent(HttpParser.ContentDelimiterString);

            // Append body, if present
            if(response.Body.Length > 0)
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                response.Body.CopyTo(content);
            }

            return content.ToArray();
        }
    }
}