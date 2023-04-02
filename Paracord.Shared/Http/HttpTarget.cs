using System.Diagnostics.CodeAnalysis;

namespace Paracord.Shared.Http
{
    /// <summary>
    /// Structure defining an HTTP-target, including both path and optional query string.
    /// </summary>
    /// <seealso href="https://datatracker.ietf.org/doc/html/rfc9112#name-request-target">Reference</seealso>
    public class HttpTarget
    {
        /// <summary>
        /// Array of all path-segments (e.g. /account/admin = ["account", "admin"]).
        /// </summary>
        public string[] PathSegments { get; set; } = default!;

        /// <summary>
        /// Get the path-segment from the request target.
        /// </summary>
        public string Path
        {
            get => "/" + string.Join("/", this.PathSegments);
        }

        /// <summary>
        /// Hashmap for the query parameters (e.g. "?q=1&a=b&q=2" = [{"q", "2"}, {"a", "b"}])
        /// </summary>
        public Dictionary<string, string> QueryParameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Parse a string (e.g. /page?q=2) to a native <c>HttpTarget</c>-instance.
        /// </summary>
        /// <param name="target">The HTTP request target to parse (e.g. /page?q=2).</param>
        /// <param name="result">The resulting <c>HttpTarget</c>-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the target was successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string target, [NotNullWhen(true)] out HttpTarget? result)
        {
            result = null;

            // Target forms other than origin-form are not yet supported.
            if(!target.StartsWith("/"))
            {
                return false;
            }

            // Remove #, if found, as it may indicate EOL.
            if(target.EndsWith("#"))
            {
                target = target.Substring(0, target.Length - 1);
            }

            // Remove the initial slash (/).
            target = target.Substring(1);

            result = new HttpTarget();

            string[] components = target.Split('?', 2);

            if(components.Length >= 1)
            {
                string pathComponents = components[0];
                result.PathSegments = pathComponents.Split('/');
            }

            if(components.Length >= 2)
            {
                string queryComponents = components[1];
                string[] queries = queryComponents.Split('&');

                foreach(var query in queries)
                {
                    string name = query.Split('=', 2)[0];
                    string value = query.Split('=', 2)[1];

                    // Empty names don't seem to be allowed, but the spec
                    // is a little ambiguous.
                    if(name.Length <= 0)
                    {
                        result = null;
                        return false;
                    }

                    result.QueryParameters.Add(name, value);
                }
            }

            return true;
        }

        /// <summary>
        /// Parse a string (e.g. /page?q=2) to a native <c>HttpTarget</c>-instance.
        /// </summary>
        /// <param name="target">The HTTP version string to parse (e.g. /page?q=2).</param>
        /// <returns>The parsed <c>HttpTarget</c>-instance.</returns>
        public static HttpTarget Parse(string target)
        {
            if(HttpTarget.TryParse(target, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied HTTP-target is improperly formatted.");
        }
    }
}