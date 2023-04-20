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
        public readonly Dictionary<string, object?> Parameters = new Dictionary<string, object?>();
    }
}