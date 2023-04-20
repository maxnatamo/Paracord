using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Constraint for matching and parsing route values, using a Regex pattern.
    /// </summary>
    public abstract class RegexRouteConstraint : IRouteConstraint
    {
        /// <inheritdoc cref="IRouteConstraint.Identifier" />
        public abstract string Identifier { get; }

        /// <summary>
        /// The Regex pattern to use for the constraint.
        /// </summary>
        public abstract string Pattern { get; }

        /// <summary>
        /// The <see cref="Regex" />-instance to use for the constraint.
        /// </summary>
        private readonly Regex RegexPattern;

        protected RegexRouteConstraint()
        {
            this.RegexPattern = new Regex(this.Pattern);
        }

        /// <summary>
        /// Try to match the specified <paramref name="value" /> using the instance's Regex pattern and return the result via <paramref name="result" />.
        /// </summary>
        /// <param name="value">The route value to try to parse.</param>
        /// <param name="result">The resulting <see cref="System.String" />, if the method returns true. Otherwise, <c>null</c>.</param>
        /// <returns>Returns <c>true</c>, if <paramref name="value" /> was parsed successfully. Otherwise, <c>false</c>.</returns>
        public bool Match(string value, [NotNullWhen(true)] out object? result)
        {
            Match match = this.RegexPattern.Match(value);

            if(!match.Success)
            {
                result = null;
                return false;
            }

            result = value;
            return true;
        }
    }
}