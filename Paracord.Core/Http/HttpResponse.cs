using System.Collections.Specialized;
using System.Text;

using Newtonsoft.Json;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Http
{
    /// <summary>
    /// Native definitions for HTTP responses.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// The HTTP response body <see cref="Stream" />.
        /// </summary>
        public Stream Body { get; set; } = default!;

        /// <summary>
        /// The length of the HTTP response body.
        /// </summary>
        public Int64 ContentLength
        {
            get => this.Body.Length;
        }

        /// <summary>
        /// The current context for the response.
        /// </summary>
        public HttpContext Context { get; set; } = default!;

        /// <summary>
        /// The HTTP cookies to send to the client.
        /// </summary>
        public NameValueCollection Cookies { get; set; }

        /// <summary>
        /// The HTTP headers sent along with the response.
        /// </summary>
        public NameValueCollection Headers { get; set; }

        /// <summary>
        /// The HTTP response code for the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Ok;

        /// <summary>
        /// The thread Id for the thread executing/handling this request.
        /// </summary>
        public int ThreadId { get; private set; }

        /// <summary>
        /// Initialize a new <see cref="HttpResponse" />-instance.
        /// </summary>
        public HttpResponse()
        {
            this.Body = new MemoryStream();

            this.Cookies = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            this.Headers = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        /// <inheritdoc cref="HttpContext.Send" />
        public void Send()
            => this.Context.Send();

        /// <summary>
        /// Writes the given binary data to the response body.
        /// </summary>
        /// <param name="buffer">The binary data array to write to the body.</param>
        /// <param name="offset">The offset into the <paramref name="buffer" />-parameter to start reading</param>
        /// <param name="count">The maximum amount of bytes to write to the body.</param>
        public void Write(byte[] buffer, int offset, int count)
            => this.Body.Write(buffer, offset, count);

        /// <summary>
        /// Writes the given text to the response body.
        /// </summary>
        /// <param name="text">The text to write to the body.</param>
        public void Write(string text)
            => this.Write(text, Encoding.ASCII);

        /// <summary>
        /// Writes the given text to the response body, given the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding to use, when converting the text to bytes.</param>
        /// <inheritdoc cref="HttpResponse.Write(string)" />
        public void Write(string text, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(text);
            this.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Write the specified object as JSON to the response body.
        /// As a side-effect, the Content-Type header will be set to <c>application/json; charset=utf-8</c>
        /// </summary>
        /// <param name="value">The value to write as JSON to the body.</param>
        /// <param name="type">The type of object to write.</param>
        /// <param name="serializerSettings">Serialzier settings to pass to the serializer.</param>
        public void WriteToJson(object value, Type type, JsonSerializerSettings serializerSettings = default!)
        {
            string jsonString = JsonConvert.SerializeObject(value, type, serializerSettings);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(jsonString);

            this.Body.Seek(0, SeekOrigin.Begin);
            this.Body.Write(jsonBytes, 0, jsonBytes.Length);

            this.Headers[HttpHeaders.ContentType] = "application/json; charset=utf-8";
        }

        /// <inheritdoc cref="HttpResponse.WriteToJson(object, Type, JsonSerializerSettings)" />
        /// <typeparam name="T">The type of object to write.</typeparam>
        public void WriteToJson<T>(T value, JsonSerializerSettings serializerSettings = default!) where T : class
            => this.WriteToJson(value, typeof(T), serializerSettings);

        /// <inheritdoc cref="HttpResponse.WriteToJson(object, Type, JsonSerializerSettings)" />
        public async Task WriteToJsonAsync(object value, Type type, JsonSerializerSettings serializerSettings = default!)
        {
            string jsonString = JsonConvert.SerializeObject(value, type, serializerSettings);
            byte[] jsonBytes = Encoding.ASCII.GetBytes(jsonString);

            this.Body.Seek(0, SeekOrigin.Begin);
            await this.Body.WriteAsync(jsonBytes, 0, jsonBytes.Length);

            this.Headers[HttpHeaders.ContentType] = "application/json; charset=utf-8";
        }

        /// <inheritdoc cref="HttpResponse.WriteToJson{T}(T, JsonSerializerSettings)" />
        public async Task WriteToJsonAsync<T>(T value, JsonSerializerSettings serializerSettings = default!) where T : class
            => await this.WriteToJsonAsync(value, typeof(T), serializerSettings);
    }
}