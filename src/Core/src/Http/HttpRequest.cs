using System.Collections.Specialized;
using System.Text;

using Newtonsoft.Json;
using Paracord.Shared.Models.Http;
using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Core.Http
{
    /// <summary>
    /// Native definitions for HTTP requests.
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// The HTTP request body Stream.
        /// </summary>
        public Stream Body { get; set; } = default!;

        /// <summary>
        /// The length of the HTTP request body.
        /// </summary>
        public Int64 ContentLength
        {
            get => this.Body.Length;
        }

        /// <summary>
        /// The current context for the request.
        /// </summary>
        public HttpContext Context { get; set; } = default!;

        /// <summary>
        /// The HTTP cookies sent along with the request.
        /// </summary>
        public NameValueCollection Cookies { get; set; }

        /// <summary>
        /// The HTTP headers sent along with the request.
        /// </summary>
        public NameValueCollection Headers { get; set; }

        /// <summary>
        /// The HTTP request method.
        /// </summary>
        public HttpMethod Method { get; set; } = default!;

        /// <summary>
        /// The HTTP request path.
        /// </summary>
        public string Path { get => this.Target.Path; }

        /// <summary>
        /// The request parameters, parsed from the route
        /// </summary>
        public Dictionary<string, object?> Parameters { get; internal set; }

        /// <summary>
        /// The HTTP request protocol (e.g. HTTP/1.1, HTTP/2.0).
        /// </summary>
        public HttpVersion Protocol { get; set; } = default!;

        /// <summary>
        /// The HTTP query parameters.
        /// </summary>
        public IDictionary<string, string> Query
        {
            get => this.Target.QueryParameters;
        }

        /// <summary>
        /// The HTTP request target.
        /// </summary>
        public HttpTarget Target { get; set; } = default!;

        /// <summary>
        /// The thread Id for the thread executing/handling this request.
        /// </summary>
        public int ThreadId { get; private set; }

        /// <summary>
        /// The date/time when the request was received, for internal measuring.
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// Initialize a new <see cref="HttpRequest" />-instance.
        /// </summary>
        public HttpRequest()
        {
            this.Body = new MemoryStream();

            this.Cookies = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            this.Headers = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);
            this.Parameters = new Dictionary<string, object?>();

            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// Checks the <c>Content-Type</c> header for whether the content should contain JSON-content.
        /// </summary>
        public bool HasJsonContent()
            => this.Headers[HttpHeaders.ContentType]?.StartsWith("application/json") ?? false;

        /// <summary>
        /// Read the content of the request as JSON and deserialize it to the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the <c>Content-Type</c> is not a valid JSON-type, an exception will be thrown.
        /// </para>
        /// <para>
        /// The position of the Body-stream is saved and reloaded, so there's no need to save it externally.
        /// </para>
        /// </remarks>
        /// <param name="type">The type of object to read and convert into.</param>
        /// <param name="serializerSettings">Serializer settings to pass to the serializer.</param>
        /// <returns>The object read from the request body.</returns>
        /// <exception cref="FormatException">Thrown if the <c>Content-Type</c> is not a valid JSON-type.</exception>
        public object? ReadFromJson(Type type, JsonSerializerSettings serializerSettings = default!)
        {
            if(!this.HasJsonContent())
            {
                throw new FormatException("Content-Type is not a valid Json-type.");
            }

            // Save position into body
            long position = this.Body.Position;

            // Allocate buffer
            Byte[] bodyContent = new Byte[this.ContentLength];

            // Read into buffer
            this.Body.Seek(0, SeekOrigin.Begin);
            int read = this.Body.Read(bodyContent, 0, (int) this.ContentLength);

            // Reload position
            this.Body.Seek(position, SeekOrigin.Begin);

            return JsonConvert.DeserializeObject(Encoding.ASCII.GetString(bodyContent), type);
        }

        /// <inheritdoc cref="HttpRequest.ReadFromJson(Type, JsonSerializerSettings)" />
        /// <typeparam name="T">The type of object to read and convert into.</typeparam>
        public T? ReadFromJson<T>(JsonSerializerSettings serializerSettings = default!) where T : class
            => this.ReadFromJson(typeof(T)) as T;

        /// <inheritdoc cref="HttpRequest.ReadFromJson(Type, JsonSerializerSettings)" />
        public async ValueTask<object?> ReadFromJsonAsync(Type type, JsonSerializerSettings serializerSettings = default!)
        {
            if(!this.HasJsonContent())
            {
                throw new FormatException("Content-Type is not a valid Json-type.");
            }

            // Save position into body
            long position = this.Body.Position;

            // Allocate buffer
            Byte[] bodyContent = new Byte[this.ContentLength];

            // Read into buffer
            this.Body.Seek(0, SeekOrigin.Begin);
            int read = await this.Body.ReadAsync(bodyContent, 0, (int) this.ContentLength);

            // Reload position
            this.Body.Seek(position, SeekOrigin.Begin);

            return JsonConvert.DeserializeObject(Encoding.ASCII.GetString(bodyContent), type);
        }

        /// <inheritdoc cref="HttpRequest.ReadFromJson{T}" />
        public async ValueTask<T?> ReadFromJsonAsync<T>(JsonSerializerSettings serializerSettings = default!) where T : class
            => await this.ReadFromJsonAsync(typeof(T)) as T;
    }
}