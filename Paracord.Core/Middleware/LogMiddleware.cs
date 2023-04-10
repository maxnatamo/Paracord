using Paracord.Core.Http;
using Paracord.Core.Listener;

namespace Paracord.Core.Middleware
{
    /// <summary>
    /// This middleware logs incoming requests to the console.
    /// </summary>
    public class LogMiddleware : MiddlewareBase
    {
        public override void OnServerStarted(HttpListener listener)
        {
            Console.WriteLine("Listening on {0}:{1}", listener.ListenAddress.ToString(), listener.ListenPort);
            this.Next();
        }

        public override void OnServerClosed(HttpListener listener)
        {
            Console.WriteLine("Closing server...");
            this.Next();
        }

        public override void BeforeResponseSent(HttpListener listener, HttpRequest request, HttpResponse response)
        {
            Console.WriteLine("{0} {1} {2}", request.Method.ToString(), request.Path, request.Protocol);
            this.Next();
        }
    }
}