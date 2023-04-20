namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Constraint for matching and parsing route values, as one or more alphabetical characters (<c>a-z</c>, case-insensitive).
    /// </summary>
    public class AlphaRouteConstraint : RegexRouteConstraint
    {
        /// <inheritdoc cref="IRouteConstraint.Identifier" />
        public override string Identifier => "alpha";

        /// <summary>
        /// The Regex pattern to use for the constraint.
        /// </summary>
        public override string Pattern => @"^[a-zA-Z]*$";
    }
}