using Paracord.Core.Http;

namespace Paracord.Core.Controller
{
    public abstract partial class ControllerBase
    {
        /// <summary>
        /// The <see cref="HttpRequest" /> for the executing action.
        /// </summary>
        public HttpRequest Request { get; internal set; } = null!;

        /// <summary>
        /// The <see cref="HttpResponse" /> for the executing action.
        /// </summary>
        public HttpResponse Response { get; internal set; } = null!;
    }
}