namespace Paracord.Core.Application
{
    /// <summary>
    /// Environment information for <see cref="WebApplication"/>-instances.
    /// </summary>
    public class WebApplicationEnvironment
    {
        /// <summary>
        /// The relative path to all statically servicable files.
        /// </summary>
        public string WebRoot { get; set; } = "wwwroot";

        /// <summary>
        /// The current environment of the <see cref="WebApplication"/>, i.e. Development, Staging, etc.
        /// </summary>
        public string Environment { get; set; } = "Development";

        /// <summary>
        /// Whether the current environment matches the one specified.
        /// </summary>
        /// <param name="environment">The value to check against.</param>
        /// <returns>True, if the two values match. Otherwise, false.</returns>
        public bool IsEnvironment(string environment)
            => this.Environment.Equals(environment);

        /// <summary>
        /// Whether the current environment is "Development".
        /// </summary>
        public bool IsDevelopment() => this.IsEnvironment("Development");

        /// <summary>
        /// Whether the current environment is "Staging".
        /// </summary>
        public bool IsStaging() => this.IsEnvironment("Staging");

        /// <summary>
        /// Whether the current environment is "Production".
        /// </summary>
        public bool IsProduction() => this.IsEnvironment("Production");
    }
}