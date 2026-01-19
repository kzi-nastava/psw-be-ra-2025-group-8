namespace Explorer.Tours.API.Dtos
{
    public class WeatherDailyForecastDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Timezone { get; set; }

        public List<WeatherDailyForecastItemDto> Days { get; set; } = new();
    }
}
