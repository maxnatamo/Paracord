using System.Reflection;

using Paracord.Core.Controller;
using Paracord.Shared.Attributes;
using Paracord.Shared.Extensions;
using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Core.Extensions
{
    public static class RouteExtensions
    {
        /// <summary>
        /// Parse the route from the given type, <paramref name="type" />, and return it.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> to parse the route from.</param>
        /// <returns>A <see cref="string" /> containing the route.</returns>
        public static string ParseRoute(this Type type)
        {
            List<RouteAttribute> routeAttributes = type.GetAttributes<RouteAttribute>();

            if(routeAttributes.Any())
            {
                return routeAttributes.First().Path;
            }

            return $"/{ControllerBase.GetControllerName(type).ToLower()}";
        }

        /// <summary>
        /// Parse the route from the given <see cref="MethodInfo" />, <paramref name="info" />, and return it.
        /// </summary>
        /// <param name="info">The <see cref="MethodInfo" /> to parse the route from.</param>
        /// <returns>A <see cref="string" /> containing the route.</returns>
        public static string ParseRoute(this MethodInfo info)
        {
            List<RouteAttribute> routeAttributes = info.GetAttributes<RouteAttribute>();

            if(routeAttributes.Any())
            {
                return routeAttributes.First().Path;
            }

            string methodName = info.Name.ToLower();
            if(methodName == "index")
            {
                methodName = "/";
            }

            return methodName;
        }

        /// <summary>
        /// Parse the HTTP method from the given <see cref="MethodInfo" />, <paramref name="info" />, and return it.
        /// </summary>
        /// <param name="info">The <see cref="MethodInfo" /> to parse the method from.</param>
        /// <returns>
        /// The parsed <see cref="HttpMethod" />, if any <see cref="HttpMethodAttribute" /> where applied.
        /// Otherwise, <see cref="HttpMethod.All" />.
        /// </returns>
        public static HttpMethod ParseHttpMethod(this MethodInfo info)
            => info.GetAttribute<HttpMethodAttribute>()?.Methods ?? HttpMethod.All;
    }
}