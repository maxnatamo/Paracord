using System.Reflection;

using Paracord.Core.Extensions;
using Paracord.Core.Parsing.Routing;

namespace Paracord.Core.Controller
{
    public partial class ControllerBase
    {
        /// <summary>
        /// Get the friendly-name of the controller type name.
        /// </summary>
        /// <param name="type">The type to parse the name from.</param>
        /// <returns>The friendly-name of the type.</returns>
        internal static string GetControllerName(Type type)
        {
            string typeName = type.Name;
            int offset = typeName.LastIndexOf("Controller", StringComparison.InvariantCultureIgnoreCase);

            if(offset == -1)
            {
                return typeName;
            }

            return typeName.Substring(0, offset);
        }

        /// <summary>
        /// Get all types from the loaded assemblies, which drive from <see cref="ControllerBase" />.
        /// </summary>
        /// <returns>List of <see cref="ControllerBase" />.</returns>
        internal static List<Type> GetAllControllers()
            => AppDomain.CurrentDomain.GetAssemblies()
                .Where(v => !v.FullName?.StartsWith("Paracord") ?? true)
                .SelectMany(v => v.GetTypes())
                .Where(v => v.IsSubclassOf(typeof(ControllerBase)))
                .ToList();

        /// <summary>
        /// Get all available routes from the specified <see cref="ControllerBase" /> type.
        /// </summary>
        /// <param name="controllerType">The controller type to parse the routes from.</param>
        /// <returns>List of <see cref="ControllerRoute" />.</returns>
        internal static IEnumerable<ControllerRoute> GetAllRoutes(Type controllerType)
        {
            List<MethodInfo> methods = controllerType
                .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(v => !v.IsSpecialName)
                .ToList();

            foreach(MethodInfo method in methods)
            {
                ControllerRoute route = ControllerBase.ParseControllerRoute(controllerType, method);
                yield return route;
            }
        }

        /// <summary>
        /// Parse a single method from a <see cref="ControllerBase" /> and return the parsed <see cref="ControllerRoute" />-instance.
        /// </summary>
        /// <param name="controllerType">The controller type to parse the routes from.</param>
        /// <param name="methodInfo">The actual method to parse.</param>
        /// <returns>The parsed <see cref="ControllerRoute" />-instance.</returns>
        internal static ControllerRoute ParseControllerRoute(Type controllerType, MethodInfo methodInfo)
        {
            RouteParser parser = new RouteParser();

            ControllerRoute route = new ControllerRoute();
            route.ParentControllerType = controllerType;
            route.HttpMethod = methodInfo.ParseHttpMethod();
            route.ExecutorMethod = methodInfo;

            string controllerRoute = controllerType.ParseRoute();
            route.ControllerPath = parser.Parse(controllerRoute);

            string methodRoute = methodInfo.ParseRoute();
            route.MethodPath = parser.Parse(methodRoute);

            return route;
        }
    }
}