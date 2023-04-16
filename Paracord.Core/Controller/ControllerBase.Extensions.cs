using System.Reflection;

using Paracord.Core.Extensions;

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
        /// <param name="controller">The controller to parse the routes from.</param>
        /// <returns>List of <see cref="ControllerRoute" />.</returns>
        internal static IEnumerable<ControllerRoute> GetAllRoutes<T>(T controller) where T : ControllerBase
        {
            List<MethodInfo> methods = controller.GetType()
                .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(v => !v.IsSpecialName)
                .ToList();

            foreach(MethodInfo method in methods)
            {
                ControllerRoute route = ControllerBase.ParseControllerRoute(controller, method);
                yield return route;
            }
        }

        /// <summary>
        /// Parse a single method from a <see cref="ControllerBase" /> and return the parsed <see cref="ControllerRoute" />-instance.
        /// </summary>
        /// <param name="controller">The controller to parse the method from.</param>
        /// <param name="methodInfo">The actual method to parse.</param>
        /// <returns>The parsed <see cref="ControllerRoute" />-instance.</returns>
        internal static ControllerRoute ParseControllerRoute<T>(T controller, MethodInfo methodInfo) where T : ControllerBase
        {
            ControllerRoute route = new ControllerRoute();
            route.ParentController = controller;
            route.ControllerPath = controller.GetType().ParseRoute();
            route.MethodPath = methodInfo.ParseRoute();
            route.HttpMethod = methodInfo.ParseHttpMethod();
            route.Executor = ctx => methodInfo.Invoke(controller, new object[] { ctx });

            return route;
        }
    }
}