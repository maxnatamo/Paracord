using Paracord.Shared.Models.Http;
using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Core.Controller
{
    /// <summary>
    /// A controller route, representing a single method on a controller.
    /// </summary>
    public class ControllerRoute
    {
        /// <summary>
        /// The parent controller class.
        /// </summary>
        public ControllerBase ParentController { get; set; } = default!;

        /// <summary>
        /// The relative path of the parent controller.
        /// </summary>
        public string ControllerPath { get; set; } = string.Empty;

        /// <summary>
        /// The relative path of the method.
        /// </summary>
        public string MethodPath { get; set; } = string.Empty;

        /// <summary>
        /// The HTTP method of the route.
        /// </summary>
        public HttpMethod HttpMethod { get; set; } = HttpMethod.GET;

        /// <summary>
        /// The full, relative path of the route.
        /// </summary>
        public string RoutePath
        {
            get => Path.Join(this.ControllerPath, this.MethodPath);
        }

        /// <summary>
        /// The actual handler on the controller.
        /// </summary>
        public Action<HttpContext> Executor { get; set; } = ctx => { };
    }
}