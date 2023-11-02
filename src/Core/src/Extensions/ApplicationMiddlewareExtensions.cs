using Paracord.Core.Application;
using Paracord.Core.Listener;

namespace Paracord.Core.Extensions
{
    public static class ApplicationMiddlewareExtensions
    {
        /// <summary>
        /// Register a new configuration for other services.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder" />-instance to add the configuration onto.</param>
        /// <param name="configureOptions">An action to configure the options.</param>
        /// <typeparam name="T">The type of the configuration to register.</typeparam>
        /// <returns>The <see cref="WebApplicationBuilder" /> to allow for method chaining.</returns>
        public static WebApplicationBuilder Configure<T>(this WebApplicationBuilder builder, Action<T> configureOptions) where T : new()
        {
            ArgumentNullException.ThrowIfNull(builder);
            ArgumentNullException.ThrowIfNull(configureOptions);

            T options = new T();

            configureOptions(options);

            builder.Services.RegisterInstance(options);
            return builder;
        }

        /// <summary>
        /// Register a new middleware onto the <see cref="WebApplicationBuilder" />-instance.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder" />-instance to add the middleware onto.</param>
        /// <typeparam name="T">The type of the middleware to add.</typeparam>
        /// <returns>The <see cref="WebApplicationBuilder" /> to allow for method chaining.</returns>
        public static WebApplicationBuilder RegisterMiddleware<T>(this WebApplicationBuilder builder) where T : MiddlewareBase
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.Register<MiddlewareBase, T>(typeof(T).Name);
            return builder;
        }
    }
}