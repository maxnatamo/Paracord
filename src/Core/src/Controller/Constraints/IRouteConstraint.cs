using System.Diagnostics.CodeAnalysis;

namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Interface for a controller route constraint, for parsing/handling variable route values.
    /// </summary>
    public interface IRouteConstraint
    {
        /// <summary>
        /// The identifier for the constraint, used in the actual routes.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Try to match the specified <paramref name="value" /> and return the result via <paramref name="result" />.
        /// </summary>
        /// <param name="value">The route value to try to parse.</param>
        /// <param name="result">The resulting object, of type decided by the constraint, if the method returns true. Otherwise, <c>null</c>.</param>
        /// <returns>Returns <c>true</c>, if <paramref name="value" /> was parsed successfully. Otherwise, <c>false</c>.</returns>
        public bool Match(string value, [NotNullWhen(true)] out object? result);
    }
}