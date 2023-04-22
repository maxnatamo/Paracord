using Paracord.Core.Application;
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
        /// <param name="application">The parent <see cref="WebApplication" />-instance.</param>
        /// <param name="request">The <see cref="HttpRequest" /> to match against.</param>
        /// <returns>The matched <see cref="ControllerRouteMatch" />-instance.</returns>
        public ControllerRouteMatch Match(WebApplication application, HttpRequest request)
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
                bool matched = this.RoutePath[i].Type switch
                {
                    ControllerRouteSegmentType.Constant => this.MatchConstantRoute(application, match, this.RoutePath[i], requestPathSegments[i]),
                    ControllerRouteSegmentType.Variable => this.MatchVariableRoute(application, match, this.RoutePath[i], requestPathSegments[i]),

                    _ => throw new ArgumentException($"Invalid ControllerRouteSegmentType: {this.RoutePath[i].Type.ToString()}")
                };

                if(!matched)
                {
                    return new ControllerRouteMatch { Success = false };
                }
            }

            return match;
        }

        /// <summary>
        /// Match a constant route segment and populate <paramref name="match" />, unless the match failed.
        /// </summary>
        /// <param name="application">The parent <see cref="WebApplication" />-instance.</param>
        /// <param name="match">The <see cref="ControllerRouteMatch" /> to fill in, if the match succeeds.</param>
        /// <param name="pattern">The controller route pattern to match against.</param>
        /// <param name="path">The input route path to match with.</param>
        /// <returns>Returns <c>true</c>, if the match succeeded. Otherwise, <c>false</c>.</returns>
        internal bool MatchConstantRoute(WebApplication application, ControllerRouteMatch match, ControllerRouteSegment pattern, string path)
            => pattern.Name.ToLower() == path.ToLower();

        /// <summary>
        /// Match a variable route segment and populate <paramref name="match" />, unless the match failed.
        /// </summary>
        /// <param name="application">The parent <see cref="WebApplication" />-instance.</param>
        /// <param name="match">The <see cref="ControllerRouteMatch" /> to fill in, if the match succeeds.</param>
        /// <param name="pattern">The controller route pattern to match against.</param>
        /// <param name="path">The input route path to match with.</param>
        /// <returns>Returns <c>true</c>, if the match succeeded. Otherwise, <c>false</c>.</returns>
        internal bool MatchVariableRoute(WebApplication application, ControllerRouteMatch match, ControllerRouteSegment pattern, string path)
        {
            IRouteConstraint? constraint = null;

            if(pattern.ConstraintName != null)
            {
                constraint = application.RouteConstraintsOptions.Constraints.FirstOrDefault(v => v.Identifier == pattern.ConstraintName);

                if(constraint == null)
                {
                    return false;
                }
            }

            // Handle default values
            if(string.IsNullOrEmpty(path))
            {
                if(pattern.Default == null)
                {
                    return false;
                }

                path = pattern.Default!;
            }

            if(!this.ValidateConstraints(constraint, path, out var parsedRouteValue))
            {
                return false;
            }

            match.Parameters.Add(pattern.Name, parsedRouteValue);
            return true;
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