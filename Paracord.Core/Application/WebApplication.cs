using LightInject;
using Paracord.Core.Controller;
using Paracord.Core.Listener;
using Paracord.Shared.Models.Http;
using Paracord.Shared.Models.Listener;

namespace Paracord.Core.Application
{
    /// <summary>
    /// Web application class for handling and routing HTTP connections.
    /// </summary>
    public class WebApplication
    {
        /// <summary>
        /// The underlying <see cref="HttpListener" />-instance.
        /// </summary>
        private readonly HttpListener Listener;

        /// <summary>
        /// Collection of routes available for the application.
        /// </summary>
        protected internal readonly ControllerRouteCollection Routes = new ControllerRouteCollection();

        /// <summary>
        /// The services available to the <see cref="WebApplication" />.
        /// </summary>
        public readonly ServiceContainer Services;

        /// <summary>
        /// List of all middlewares used by the listener.
        /// </summary>
        protected IEnumerable<MiddlewareBase> Middlewares => this.Services.GetAllInstances<MiddlewareBase>();

        /// <summary>
        /// Environment information for the <see cref="WebApplication"/>-instance.
        /// </summary>
        public readonly WebApplicationEnvironment Environment;

        /// <summary>
        /// Runtime options for the <see cref="WebApplication"/>-instance.
        /// </summary>
        protected readonly WebApplicationOptions Options;

        public WebApplication(ServiceContainer services, WebApplicationEnvironment environment, WebApplicationOptions options)
        {
            this.Listener = new HttpListener();
            this.Services = services;
            this.Environment = environment;
            this.Options = options;

            if(!this.Options.Prefixes.Any())
            {
                this.Options.Prefixes.Add("http://0.0.0.0:8080");

                if(this.Options.SslCertificate != null)
                {
                    this.Options.Prefixes.Add("https://0.0.0.0:8443");
                }
            }

            foreach(ListenerPrefix prefix in this.Options.Prefixes)
            {
                this.Listener.Prefixes.Add(prefix);
            }
        }

        /// <summary>
        /// Start the <see cref="WebApplication"/> and start listening for connections.
        /// </summary>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> for stopping the application.</param>
        /// <returns>A <see cref="Task" /> representing the applications runtime.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default!)
        {
            await Task.Run(() =>
            {
                this.Listener.Start();
                this.ExecuteMiddleware(_ => _.OnServerStarted(this.Listener));

                this.Listener.AcceptConnections(ctx =>
                {
                    this.ExecuteMiddleware(_ => _.AfterRequestReceived(this.Listener, ctx.Request, ctx.Response));

                    // Call context handler
                    this.ContextHandler(ctx);

                    this.ExecuteMiddleware(_ => _.BeforeResponseSent(this.Listener, ctx.Request, ctx.Response));
                },
                cancellationToken);
            });
        }

        /// <summary>
        /// Stop the <see cref="WebApplication"/> from listening.
        /// </summary>
        public void Stop()
        {
            this.ExecuteMiddleware(_ => _.OnServerClosed(this.Listener));
            this.Listener.Stop();
        }

        /// <summary>
        /// Handle the specified <see cref="HttpContext" /> and execute the appropriate handler.
        /// </summary>
        /// <param name="ctx">The <see cref="HttpContext" />-instance to handle.</param>
        internal void ContextHandler(HttpContext ctx)
        {
            ControllerRoute? route = this.Routes.ParseRequestPath(ctx.Request);
            if(route == null)
            {
                ctx.Response.StatusCode = HttpStatusCode.NotFound;
                ctx.Send();

                return;
            }

            using(this.Services.BeginScope())
            {
                ControllerBase controller = (ControllerBase) this.Services.GetInstance(route.ParentControllerType);
                controller.Request = ctx.Request;
                controller.Response = ctx.Response;

                object? result = route.ExecutorMethod.Invoke(controller, new object[] { });

                if(ctx.IsOpen)
                {
                    if(result != null)
                    {
                        ctx.Response.Body.SetLength(0);
                        ctx.Response.WriteToJson(result, route.ExecutorMethod.ReturnType);
                    }

                    ctx.Send();
                }
            }
        }

        /// <summary>
        /// Execute a middleware method, selected by the specified selector.
        /// </summary>
        /// <param name="runner">Selects the method to run on the middlewares.</param>
        /// <param name="reversed">Whether to reverse the middleware ordering.</param>
        /// <example>
        /// For example, to execute the <see cref="MiddlewareBase.OnServerStarted(HttpListener)" />-method in all the middleware:
        /// <code>
        /// this.ExecuteMiddleware(_ => _.OnServerStarted(this));
        /// </code>
        /// </example>
        internal void ExecuteMiddleware(Action<MiddlewareBase> runner, bool reversed = false)
        {
            var middlewares = this.Middlewares;

            // Useful for response-middleware.
            if(reversed)
            {
                middlewares.Reverse();
            }

            foreach(var middleware in this.Middlewares)
            {
                runner(middleware);

                if(!middleware.UseNextMiddleware)
                {
                    break;
                }
            }
        }
    }
}