using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
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
        /// <param name="request">The <see cref="HttpRequest" /> to query for.</param>
        /// <returns>The matching <see cref="ControllerRoute" />, if found. Otherwise, null.</returns>
        public ControllerRoute? ParseRequestPath(HttpRequest request)
        {
            foreach(ControllerRoute route in this)
            {
                ControllerRouteMatch match = route.Match(request);

                if(!match.Success)
                {
                    continue;
                }

                return route;
            }

            return null;
        }
    }
}