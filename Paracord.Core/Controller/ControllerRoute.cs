using System.Diagnostics.CodeAnalysis;
using Paracord.Core.Http;
using Paracord.Core.Parsing.Routing;
using Paracord.Shared.Exceptions;
using Paracord.Shared.Models.Http;
using HttpMethod = Paracord.Shared.Models.Http.HttpMethod;

namespace Paracord.Core.Controller
{
    /// <summary>
    /// A controller route, representing a single method on a controller.
    /// </summary>
    public class ControllerRoute
    {
        /// <summary>
        /// The parent controller class.
        /// </summary>
        public ControllerBase ParentController { get; set; } = default!;

        /// <summary>
        /// The relative path of the parent controller.
        /// </summary>
        public List<ControllerRouteSegment> ControllerPath { get; set; } = new List<ControllerRouteSegment>();

        /// <summary>
        /// The relative path of the method.
        /// </summary>
        public List<ControllerRouteSegment> MethodPath { get; set; } = new List<ControllerRouteSegment>();

        /// <summary>
        /// The HTTP method of the route.
        /// </summary>
        public HttpMethod HttpMethod { get; set; } = HttpMethod.GET;

        /// <summary>
        /// The full, relative path of the route.
        /// </summary>
        public List<ControllerRouteSegment> RoutePath
        {
            get
            {
                List<ControllerRouteSegment> segments = new List<ControllerRouteSegment>();
                segments.AddRange(this.ControllerPath);
                segments.AddRange(this.MethodPath);
                return segments;
            }
        }

        /// <summary>
        /// The actual handler on the controller.
        /// </summary>
        public Action<HttpContext> Executor { get; set; } = ctx => { };

        /// <summary>
        /// Try to parse a controller- and method route into a native <see cref="ControllerRoute" />-instance.
        /// </summary>
        /// <param name="controllerRoute">The route parameter for the matching controller.</param>
        /// <param name="methodRoute">The route parameter for the matching method on the controller.</param>
        /// <param name="result">The resulting <see cref="ControllerRoute" />-instance, if the method returns true. Otherwise, false.</param>
        /// <returns>True, if the routes were successfully parsed. Otherwise, false.</returns>
        public static bool TryParse(string controllerRoute, string methodRoute, [NotNullWhen(true)] out ControllerRoute? result)
        {
            result = new ControllerRoute();

            try
            {
                RouteParser parser = new RouteParser();
                result.ControllerPath = parser.Parse(controllerRoute);
                result.MethodPath = parser.Parse(methodRoute);
            }
            catch(UnexpectedTokenException)
            {
                result = null;
                return false;
            }

            return true;
        }

        /// <inheritdoc cref="ControllerRoute.TryParse(string, out ControllerRoute?)" />
        /// <returns>The parsed <see cref="ControllerRoute" />-instance.</returns>
        /// <exception cref=""></exception>
        public static ControllerRoute Parse(string controllerRoute, string methodRoute)
        {
            if(ControllerRoute.TryParse(controllerRoute, methodRoute, out var result))
            {
                return result;
            }
            throw new FormatException("The supplied route(s) are improperly formatted.");
        }

        /// <summary>
        /// Match the specified request-path with the <see cref="ControllerRoute" /> and return the result.
        /// <para>
        /// If the match was a success, <see cref="ControllerRouteMatch.Success" /> will be set to <c>true</c>.
        /// </para>
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest" /> to match against.</param>
        /// <returns>The matched <see cref="ControllerRouteMatch" />-instance.</returns>
        public ControllerRouteMatch Match(HttpRequest request)
        {
            string[] requestPathSegments = request.Path.Trim('/').Split('/');
            List<ControllerRouteSegment> routePath = this.RoutePath;

            ControllerRouteMatch match = new ControllerRouteMatch { Success = true };

            if(!this.HttpMethod.HasFlag(request.Method))
            {
                return new ControllerRouteMatch { Success = false };
            }

            if(requestPathSegments.Count() > routePath.Count())
            {
                return new ControllerRouteMatch { Success = false };
            }

            for(int i = 0; i < requestPathSegments.Count(); i++)
            {
                if(routePath[i].Type == ControllerRouteSegmentType.Constant && routePath[i].Name.ToLower() != requestPathSegments[i].ToLower())
                {
                    return new ControllerRouteMatch { Success = false };
                }

                if(routePath[i].Type == ControllerRouteSegmentType.Variable)
                {
                    if(string.IsNullOrEmpty(requestPathSegments[i]))
                    {
                        if(routePath[i].Default == null)
                        {
                            return new ControllerRouteMatch { Success = false };
                        }
                        else
                        {
                            requestPathSegments[i] = routePath[i].Default!;
                        }
                    }

                    match.Parameters.Add(routePath[i].Name, requestPathSegments[i]);
                }
            }

            return match;
        }
    }
}