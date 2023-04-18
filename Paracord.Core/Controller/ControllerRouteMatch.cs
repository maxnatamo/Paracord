namespace Paracord.Core.Controller
{
    /// <summary>
    /// Represents a single match between a <see cref="ControllerRoute" /> and a request path.
    /// </summary>
    public class ControllerRouteMatch
    {
        /// <summary>
        /// Whether the match was a success or not.
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Lookup table for parameters in the request route.
        /// </summary>
        /// <typeparam name="string">Name and/or key of the route parameter.</typeparam>
        /// <typeparam name="string">The actual value of the key.</typeparam>
        public readonly Dictionary<string, string> Parameters = new Dictionary<string, string>();
    }
}