using System.Diagnostics.CodeAnalysis;

namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Constraint for matching and parsing route values of type <see cref="System.Boolean" />.
    /// </summary>
    public class BooleanRouteConstraint : IRouteConstraint
    {
        /// <inheritdoc cref="IRouteConstraint.Identifier" />
        public string Identifier => "int";

        /// <summary>
        /// Try to match the specified <paramref name="value" /> as a <see cref="System.Boolean" /> and return the result via <paramref name="result" />.
        /// </summary>
        /// <param name="value">The route value to try to parse.</param>
        /// <param name="result">The resulting <see cref="System.Boolean" />, if the method returns true. Otherwise, <c>null</c>.</param>
        /// <returns>Returns <c>true</c>, if <paramref name="value" /> was parsed successfully. Otherwise, <c>false</c>.</returns>
        public bool Match(string value, [NotNullWhen(true)] out object? result)
        {
            if(!Boolean.TryParse(value, out var _result))
            {
                result = null;
                return false;
            }

            result = _result;
            return true;
        }
    }
}