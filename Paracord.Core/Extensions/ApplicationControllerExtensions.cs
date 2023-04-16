using Paracord.Core.Application;
using Paracord.Core.Controller;

namespace Paracord.Core.Extensions
{
    public static class ApplicationControllerExtensions
    {
        /// <summary>
        /// Register a new middleware onto the <see cref="WebApplication" />-instance.
        /// </summary>
        /// <param name="application">The <see cref="WebApplication" />-instance to map the controllers to.</param>
        /// <returns>The <see cref="WebApplication" /> to allow for method chaining.</returns>
        public static WebApplication MapControllers(this WebApplication application)
        {
            if(application.Services.GetAllInstances(typeof(ControllerBase)).Any())
            {
                return application;
            }

            foreach(Type controllerType in ControllerBase.GetAllControllers())
            {
                application.Services.Register(
                    typeof(ControllerBase),
                    controllerType,
                    controllerType.FullName ?? controllerType.Name
                );
            }

            foreach(ControllerBase controller in application.Services.GetAllInstances(typeof(ControllerBase)))
            {
                foreach(ControllerRoute route in ControllerBase.GetAllRoutes(controller))
                {
                    application.Routes.Add(route);
                }
            }

            return application;
        }
    }
}