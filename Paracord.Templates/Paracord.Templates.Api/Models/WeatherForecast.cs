namespace ApplicationName.Models
{
    public class WeatherForecast
    {
        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public float TemperatureCelcius { get; set; }

        public float TemperatureFahrenheit
        {
            get => (this.TemperatureCelcius * 1.8f) + 32;
        }
    }
}