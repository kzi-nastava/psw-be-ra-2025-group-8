namespace Explorer.Tours.API.Dtos
{
    public class WeatherDailyForecastItemDto
    {
        public DateTime Date { get; set; }

        // Open-Meteo weather_code (WMO codes). Frontend can map these to icons/text.
        public int WeatherCode { get; set; }

        public double TemperatureMaxC { get; set; }
        public double TemperatureMinC { get; set; }

        // 0-100
        public int? PrecipitationProbabilityMax { get; set; }
    }
}
