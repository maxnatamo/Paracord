namespace Paracord.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// The path to match for the route.
        /// </summary>
        public string Path { get; protected set; } = "/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Path">The path to match for the route.</param>
        public RouteAttribute(string Path)
        {
            this.Path = Path;
        }
    }
}