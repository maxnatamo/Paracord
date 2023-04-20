using LightInject;
using Paracord.Core.Application;
using Paracord.Core.Controller;

namespace Paracord.Core.Extensions
{
    public static class ApplicationControllerExtensions
    {
        /// <summary>
        /// Map all available controllers onto the specified <see cref="WebApplication" />-instance.
        /// </summary>
        /// <param name="application">The <see cref="WebApplication" />-instance to map the controllers to.</param>
        /// <returns>The <see cref="WebApplication" /> to allow for method chaining.</returns>
        public static WebApplication MapControllers(this WebApplication application)
        {
            if(application.Services.GetAllInstances(typeof(ControllerBase)).Any())
            {
                return application;
            }

            List<Type> controllerTypes = ControllerBase.GetAllControllers();

            foreach(Type controllerType in controllerTypes)
            {
                application.Services.Register(controllerType);
            }

            foreach(Type controllerType in controllerTypes)
            {
                foreach(ControllerRoute route in ControllerBase.GetAllRoutes(controllerType))
                {
                    application.Routes.Add(route);
                }
            }

            return application;
        }
    }
}