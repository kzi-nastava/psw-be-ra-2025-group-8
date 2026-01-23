using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Weather
{
    public interface IWeatherForecastService
    {
        WeatherDailyForecastDto GetDailyForecast(double latitude, double longitude, int days = 7);

        WeatherCurrentDto GetCurrentWeather(double latitude, double longitude);
        WeatherHourlyForecastDto GetHourlyForecast(double latitude, double longitude, int hours = 6);

    }
}
