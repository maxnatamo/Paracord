using Paracord.Core.Controller.Constraints;
using Paracord.Core.Http;

namespace Paracord.Core.Controller
{
    /// <summary>
    /// A controller route, representing a single method on a controller.
    /// </summary>
    public partial class ControllerRoute
    {
        /// <summary>
        /// Match the specified request-path with the <see cref="ControllerRoute" /> and return the result.
        /// <para>
        /// If the match was a success, <see cref="ControllerRouteMatch.Success" /> will be set to <c>true</c>.
        /// </para>
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest" /> to match against.</param>
        /// <param name="routeConstraints">List of available <see cref="IRouteConstraint" /> services.</param>
        /// <returns>The matched <see cref="ControllerRouteMatch" />-instance.</returns>
        public ControllerRouteMatch Match(HttpRequest request, IEnumerable<IRouteConstraint> routeConstraints)
        {
            string[] requestPathSegments = request.Path.Trim('/').Split('/');
            List<ControllerRouteSegment> routePath = this.RoutePath;

            ControllerRouteMatch match = new ControllerRouteMatch { Success = true };

            // Try to match HTTP method
            if(!this.HttpMethod.HasFlag(request.Method))
            {
                return new ControllerRouteMatch { Success = false };
            }

            // Try to match amount of route segments
            if(requestPathSegments.Count() != routePath.Count())
            {
                return new ControllerRouteMatch { Success = false };
            }

            // Handle individual route segments
            for(int i = 0; i < requestPathSegments.Count(); i++)
            {
                // Try to match constant values
                if(routePath[i].Type == ControllerRouteSegmentType.Constant && routePath[i].Name.ToLower() != requestPathSegments[i].ToLower())
                {
                    return new ControllerRouteMatch { Success = false };
                }

                // Handle variable routes
                if(routePath[i].Type == ControllerRouteSegmentType.Variable)
                {
                    string routeKey = routePath[i].Name;
                    string routeValue = requestPathSegments[i];
                    IRouteConstraint? constraint = null;

                    // Handle route constraints
                    if(routePath[i].ConstraintName != null)
                    {
                        constraint = routeConstraints.FirstOrDefault(v => v.Identifier == routePath[i].ConstraintName);

                        if(constraint == null)
                        {
                            return new ControllerRouteMatch { Success = false };
                        }
                    }

                    // Handle default values
                    if(string.IsNullOrEmpty(routeValue))
                    {
                        if(routePath[i].Default == null)
                        {
                            return new ControllerRouteMatch { Success = false };
                        }
                        else
                        {
                            routeValue = routePath[i].Default!;
                        }
                    }

                    if(!this.ValidateConstraints(constraint, routeValue, out var parsedRouteValue))
                    {
                        return new ControllerRouteMatch { Success = false };
                    }

                    match.Parameters.Add(routeKey, parsedRouteValue);
                }
            }

            return match;
        }

        /// <summary>
        /// Validate the <paramref name="value" /> against the specified <see cref="IRouteConstraint" /> and return the match.
        /// </summary>
        /// <remarks>
        /// If <paramref name="constraint" /> is <c>null</c>, <paramref name="result" /> is set to <paramref name="value" /> and the method returns <c>true</c>.
        /// </remarks>
        /// <param name="constraint">The constraint to match the <paramref name="value" /> against.</param>
        /// <param name="value">The actual value to try and match.</param>
        /// <param name="result">The resulting matched object.</param>
        /// <returns><c>true</c>, if the constraint was matched successfully, or the constraint is <c>null</c>. Otherwise, <c>false</c>.</returns>
        internal bool ValidateConstraints(IRouteConstraint? constraint, string value, out object? result)
        {
            if(constraint == null)
            {
                result = value;
                return true;
            }

            return constraint.Match(value, out result);
        }
    }
}