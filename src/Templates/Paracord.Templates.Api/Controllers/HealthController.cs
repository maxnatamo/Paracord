using Paracord.Core.Controller;
using Paracord.Shared.Attributes;
using Paracord.Shared.Models.Http;

namespace ApplicationName.Controllers
{
    [Route("")]
    public class HealthController : ControllerBase
    {
        [Route("health")]
        public void Health(HttpContext ctx)
            => ctx.Response.StatusCode = HttpStatusCode.Ok;
    }
}