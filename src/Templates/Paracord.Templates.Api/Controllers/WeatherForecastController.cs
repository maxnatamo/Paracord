using ApplicationName.Models;

using Paracord.Core.Controller;
using Paracord.Shared.Attributes;
using Paracord.Shared.Models.Http;

namespace ApplicationName.Controllers
{
    [Route("Weather")]
    public class WeatherForecastController : ControllerBase
    {
        public void Index(HttpContext ctx)
        {
            WeatherForecast forecast = new WeatherForecast
            {
                Longitude = ((float) Random.Shared.NextDouble() * 720 - 360),
                Latitude = ((float) Random.Shared.NextDouble() * 720 - 360),
                TemperatureCelcius = Random.Shared.Next(-12, 36),
            };

            ctx.Response.WriteToJson(forecast);
        }
    }
}