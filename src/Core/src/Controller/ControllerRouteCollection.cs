using System.Collections.ObjectModel;

using Paracord.Core.Application;
using Paracord.Core.Controller.Constraints;
using Paracord.Core.Http;

namespace Paracord.Core.Controller
{
    /// <summary>
    /// Collection of <see cref="ControllerRoute" />-instances.
    /// </summary>
    public class ControllerRouteCollection : Collection<ControllerRoute>
    {
        /// <summary>
        /// Iterate through the collection and find the first that matches the specified <see cref="HttpRequest" />-instance.
        /// </summary>
        /// <param name="application">The parent <see cref="WebApplication" />-instance.</param>
        /// <param name="request">The <see cref="HttpRequest" /> to query for.</param>
        /// <returns>The matching <see cref="ControllerRoute" />, if found. Otherwise, null.</returns>
        public ControllerRoute? ParseRequestPath(WebApplication application, HttpRequest request)
        {
            foreach(ControllerRoute route in this)
            {
                ControllerRouteMatch match = route.Match(application, request);

                if(!match.Success)
                {
                    continue;
                }

                request.Parameters = match.Parameters;
                return route;
            }

            return null;
        }
    }
}