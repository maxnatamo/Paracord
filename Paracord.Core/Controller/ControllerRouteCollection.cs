using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Paracord.Core.Controller
{
    /// <summary>
    /// Collection of <see cref="ControllerRoute" />-instances.
    /// </summary>
    public class ControllerRouteCollection : Collection<ControllerRoute>
    {
        /// <summary>
        /// Adds a <see cref="ControllerRoute" /> to the end of the <see cref="ControllerRouteCollection" />.
        /// </summary>
        /// <param name="item">The <see cref="ControllerRoute" />-instance to add.</param>
        /// <exception cref="ArgumentException">
        /// Thrown if the <see cref="ControllerRoute.RoutePath" /> of the <paramref name="item" /> is not valid Regex.
        /// </exception>
        public new void Add(ControllerRoute item)
        {
            try
            {
                new Regex(item.RoutePath);
            }
            catch(ArgumentException e)
            {
                throw new ArgumentException($"Route combination is not valid Regex: {item.RoutePath}", e);
            }
        }
    }
}