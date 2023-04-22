using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Constraint for matching and parsing route values of type <see cref="System.Guid" />.
    /// </summary>
    public class GuidRouteConstraint : IRouteConstraint
    {
        /// <inheritdoc cref="IRouteConstraint.Identifier" />
        public string Identifier => "guid";

        /// <summary>
        /// Try to match the specified <paramref name="value" /> as a <see cref="System.Guid" /> and return the result via <paramref name="result" />.
        /// </summary>
        /// <remarks>
        /// Only strings using the <c>N</c> and <c>D</c> <see cref="System.Guid" /> formats are allowed.
        /// </remarks>
        /// <param name="value">The route value to try to parse.</param>
        /// <param name="result">The resulting <see cref="System.Guid" />, if the method returns true. Otherwise, <c>null</c>.</param>
        /// <returns>Returns <c>true</c>, if <paramref name="value" /> was parsed successfully. Otherwise, <c>false</c>.</returns>
        public bool Match(string value, [NotNullWhen(true)] out object? result)
        {
            Guid _result;

            if(Guid.TryParseExact(value, "N", out _result))
            {
                result = _result;
                return true;
            }

            if(Guid.TryParseExact(value, "D", out _result))
            {
                result = _result;
                return true;
            }

            result = null;
            return false;
        }
    }
}