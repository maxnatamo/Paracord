using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class HttpMethodAttribute : Attribute
    {
        /// <summary>
        /// The HTTP method(s) to match for the route.
        /// </summary>
        public HttpMethod Methods { get; protected set; } = HttpMethod.GET;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Methods">The method</param>
        public HttpMethodAttribute(HttpMethod Methods)
        {
            this.Methods = Methods;
        }
    }
}