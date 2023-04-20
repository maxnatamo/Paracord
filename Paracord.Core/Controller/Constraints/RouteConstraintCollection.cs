using System.Collections.ObjectModel;

namespace Paracord.Core.Controller.Constraints
{
    /// <summary>
    /// Collection of available <see cref="IRouteConstraint" />-instances, available to the service.
    /// </summary>
    public class RouteConstraintCollection : Collection<IRouteConstraint>
    {
        /// <summary>
        /// Adds a type representing a <see cref="IRouteConstraint" />-instance.
        /// </summary>
        /// <typeparam name="T">Type representing a <see cref="IRouteConstraint" />-instance.</typeparam>
        public void Add<T>() where T : IRouteConstraint, new()
            => this.Add(new T());
    }
}