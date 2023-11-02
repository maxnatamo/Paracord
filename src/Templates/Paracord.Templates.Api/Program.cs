using Paracord.Core.Application;
using Paracord.Core.Extensions;
using Paracord.Core.Middleware;

namespace ApplicationName
{
    class Program
    {
        static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = new WebApplicationBuilder(args);

            // Register services here
            builder.RegisterMiddleware<LogMiddleware>();
            builder.RegisterMiddleware<CompressionMiddleware>();

            WebApplication app = builder.Build();

            app.MapControllers();

            await app.StartAsync();
        }
    }
}