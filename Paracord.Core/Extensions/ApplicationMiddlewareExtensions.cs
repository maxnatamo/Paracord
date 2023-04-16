using Paracord.Core.Application;
using Paracord.Core.Listener;

namespace Paracord.Core.Extensions
{
    public static class ApplicationMiddlewareExtensions
    {
        /// <summary>
        /// Register a new middleware onto the <see cref="WebApplicationBuilder" />-instance.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder" />-instance to add the middleware onto.</param>
        /// <typeparam name="T">The type of the middleware to add.</typeparam>
        /// <returns>The <see cref="WebApplicationBuilder" /> to allow for method chaining.</returns>
        public static WebApplicationBuilder RegisterMiddleware<T>(this WebApplicationBuilder builder) where T : MiddlewareBase
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.Services.Register<MiddlewareBase, T>();
            return builder;
        }
    }
}