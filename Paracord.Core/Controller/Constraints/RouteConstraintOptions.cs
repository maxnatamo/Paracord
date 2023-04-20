namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Options for configuring route constraints.
    /// </summary>
    public class RouteConstraintOptions
    {
        /// <summary>
        /// All available route constraints.
        /// </summary>
        public readonly RouteConstraintCollection Constraints = new RouteConstraintCollection
        {
            new AlphaRouteConstraint(),
            new BooleanRouteConstraint(),
            new DateTimeRouteConstraint(),
            new DecimalRouteConstraint(),
            new DoubleRouteConstraint(),
            new FloatRouteConstraint(),
            new GuidRouteConstraint(),
            new IntegerRouteConstraint(),
            new LongRouteConstraint(),
            new StringRouteConstraint(),
        };
    }
}