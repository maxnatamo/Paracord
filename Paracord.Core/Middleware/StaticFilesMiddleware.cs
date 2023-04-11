using Paracord.Core.Http;
using Paracord.Core.Listener;
using Paracord.Shared.Models;
using Paracord.Shared.Models.Http;

namespace Paracord.Core.Middleware
{
    public class StaticFilesOptions
    {
        /// <summary>
        /// The path for the static content, relative to the executing binary.
        /// </summary>
        public string ContentPath { get; set; } = "wwwroot";
    }

    /// <summary>
    /// This middleware handles static files that might be present on the server.
    /// </summary>
    public class StaticFilesMiddleware : MiddlewareBase
    {
        /// <inheritdoc cref="StaticFilesOptions.ContentPath" />
        public readonly string ContentPath = "wwwroot";

        public StaticFilesMiddleware()
        { }

        public StaticFilesMiddleware(StaticFilesOptions options = default!)
        {
            this.ContentPath = options.ContentPath;
        }

        public override void AfterRequestReceived(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            string localPath = request.Path.StartsWith("/") ? request.Path.Substring(1) : request.Path;
            string absoluteRequestPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ContentPath, localPath);

            if(!File.Exists(absoluteRequestPath))
            {
                this.Next();
                return;
            }

            byte[] fileContent = File.ReadAllBytes(absoluteRequestPath);

            response.Headers[HttpHeaders.ContentType] = MimeTypes.ResolveMimeType(localPath);
            response.Write(fileContent, 0, fileContent.Length);
            response.Send();
        }
    }
}