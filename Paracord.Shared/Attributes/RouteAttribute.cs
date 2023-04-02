using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// The path to match for the route.
        /// </summary>
        public string Path { get; protected set; } = "/";

        /// <summary>
        /// The HTTP method(s) to match for the route.
        /// </summary>
        public HttpMethod Methods { get; protected set; } = HttpMethod.GET;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path">The path to match for the route.</param>
        /// <param name="Methods">The method</param>
        public RouteAttribute(
            string Path,
            HttpMethod Methods = HttpMethod.GET)
        {
            this.Path = Path;
            this.Methods = Methods;
        }
    }
}