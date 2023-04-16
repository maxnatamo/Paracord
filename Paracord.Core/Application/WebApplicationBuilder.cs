using LightInject;

namespace Paracord.Core.Application
{
    /// <summary>
    /// Builder pattern class for creating <see cref="WebApplication" />-instances.
    /// </summary>
    public class WebApplicationBuilder
    {
        /// <summary>
        /// The services available to the <see cref="WebApplication" />.
        /// </summary>
        public readonly ServiceContainer Services;

        /// <summary>
        /// Environment information for the <see cref="WebApplication"/>-instance.
        /// </summary>
        public readonly WebApplicationEnvironment Environment;

        /// <summary>
        /// Runtime options for the <see cref="WebApplication"/>-instance.
        /// </summary>
        public readonly WebApplicationOptions Options;

        public WebApplicationBuilder(string[] args)
        {
            this.Services = new ServiceContainer();
            this.Environment = new WebApplicationEnvironment();
            this.Options = new WebApplicationOptions();
        }

        /// <summary>
        /// Build the <see cref="WebApplication"/>-instance and return it.
        /// </summary>
        /// <returns>The built <see cref="WebApplication"/>-instance.</returns>
        public WebApplication Build()
            => new WebApplication(this.Services, this.Environment, this.Options);
    }
}