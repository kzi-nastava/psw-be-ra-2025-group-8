using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Weather;

namespace Explorer.Tours.Infrastructure.Weather
{
    public class OpenMeteoWeatherForecastService : IWeatherForecastService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        public OpenMeteoWeatherForecastService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public WeatherDailyForecastDto GetDailyForecast(double latitude, double longitude, int days = 7)
        {
            if (days < 1 || days > 16)
                throw new ArgumentException("days must be between 1 and 16.");

            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);

            // https://open-meteo.com/en/docs
            var url = $"/v1/forecast?latitude={lat}&longitude={lon}" +
                      "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max" +
                      $"&forecast_days={days}&timezone=auto";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var payload = response.Content
                .ReadFromJsonAsync<OpenMeteoForecastResponse>(JsonOptions)
                .GetAwaiter().GetResult();

            if (payload?.Daily?.Time == null)
                throw new Exception("Weather provider returned an unexpected response.");

            var result = new WeatherDailyForecastDto
            {
                Latitude = payload.Latitude,
                Longitude = payload.Longitude,
                Timezone = payload.Timezone
            };

            var n = payload.Daily.Time.Length;
            for (var i = 0; i < n; i++)
            {
                var date = DateTime.ParseExact(payload.Daily.Time[i], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                var item = new WeatherDailyForecastItemDto
                {
                    Date = date,
                    WeatherCode = SafeAt(payload.Daily.WeatherCode, i) ?? 0,
                    TemperatureMaxC = SafeAt(payload.Daily.Temperature2mMax, i) ?? 0,
                    TemperatureMinC = SafeAt(payload.Daily.Temperature2mMin, i) ?? 0,
                    PrecipitationProbabilityMax = SafeAt(payload.Daily.PrecipitationProbabilityMax, i)
                };

                result.Days.Add(item);
            }

            return result;
        }

        public WeatherCurrentDto GetCurrentWeather(double latitude, double longitude)
        {
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"/v1/forecast?latitude={lat}&longitude={lon}" +
                      "&current=temperature_2m,weather_code,wind_speed_10m" +
                      "&timezone=auto";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var payload = response.Content
                .ReadFromJsonAsync<OpenMeteoForecastResponse>(JsonOptions)
                .GetAwaiter().GetResult();

            if (payload?.Current == null)
                throw new Exception("Weather provider returned an unexpected response.");

            return new WeatherCurrentDto
            {
                Latitude = payload.Latitude,
                Longitude = payload.Longitude,
                Timezone = payload.Timezone,
                Time = DateTime.Parse(payload.Current.Time, CultureInfo.InvariantCulture),
                WeatherCode = payload.Current.WeatherCode,
                TemperatureC = payload.Current.Temperature2m,
                WindSpeedKmh = payload.Current.WindSpeed10m
            };
        }

        public WeatherHourlyForecastDto GetHourlyForecast(double latitude, double longitude, int hours = 6)
        {
            if (hours < 1 || hours > 48)
                throw new ArgumentException("hours must be between 1 and 48.");

            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"/v1/forecast?latitude={lat}&longitude={lon}" +
                      "&hourly=temperature_2m,precipitation_probability,weather_code,wind_speed_10m" +
                      $"&forecast_hours={hours}&timezone=auto";

            var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            var payload = response.Content
                .ReadFromJsonAsync<OpenMeteoForecastResponse>(JsonOptions)
                .GetAwaiter().GetResult();

            if (payload?.Hourly?.Time == null)
                throw new Exception("Weather provider returned an unexpected response.");

            var result = new WeatherHourlyForecastDto
            {
                Latitude = payload.Latitude,
                Longitude = payload.Longitude,
                Timezone = payload.Timezone
            };

            var n = payload.Hourly.Time.Length;
            for (var i = 0; i < n; i++)
            {
                result.Hours.Add(new WeatherHourlyForecastItemDto
                {
                    Time = DateTime.Parse(payload.Hourly.Time[i], CultureInfo.InvariantCulture),
                    WeatherCode = SafeAt(payload.Hourly.WeatherCode, i) ?? 0,
                    TemperatureC = SafeAt(payload.Hourly.Temperature2m, i) ?? 0,
                    PrecipitationProbability = SafeAt(payload.Hourly.PrecipitationProbability, i),
                    WindSpeedKmh = SafeAt(payload.Hourly.WindSpeed10m, i)
                });
            }

            return result;
        }


        private static int? SafeAt(int[]? arr, int i) => arr != null && i >= 0 && i < arr.Length ? arr[i] : null;
        private static double? SafeAt(double[]? arr, int i) => arr != null && i >= 0 && i < arr.Length ? arr[i] : null;

        private sealed class OpenMeteoForecastResponse
        {
            [JsonPropertyName("latitude")] public double Latitude { get; set; }
            [JsonPropertyName("longitude")] public double Longitude { get; set; }
            [JsonPropertyName("timezone")] public string? Timezone { get; set; }

            [JsonPropertyName("daily")] public OpenMeteoDaily? Daily { get; set; }
            [JsonPropertyName("current")] public OpenMeteoCurrent? Current { get; set; }
            [JsonPropertyName("hourly")] public OpenMeteoHourly? Hourly { get; set; }

        }

        private sealed class OpenMeteoDaily
        {
            [JsonPropertyName("time")] public string[] Time { get; set; } = Array.Empty<string>();
            [JsonPropertyName("weather_code")] public int[]? WeatherCode { get; set; }

            [JsonPropertyName("temperature_2m_max")] public double[]? Temperature2mMax { get; set; }
            [JsonPropertyName("temperature_2m_min")] public double[]? Temperature2mMin { get; set; }

            [JsonPropertyName("precipitation_probability_max")] public int[]? PrecipitationProbabilityMax { get; set; }
        }

        private sealed class OpenMeteoCurrent
        {
            [JsonPropertyName("time")] public string Time { get; set; } = "";
            [JsonPropertyName("temperature_2m")] public double Temperature2m { get; set; }
            [JsonPropertyName("weather_code")] public int WeatherCode { get; set; }
            [JsonPropertyName("wind_speed_10m")] public double? WindSpeed10m { get; set; }
        }

        private sealed class OpenMeteoHourly
        {
            [JsonPropertyName("time")] public string[] Time { get; set; } = Array.Empty<string>();
            [JsonPropertyName("temperature_2m")] public double[]? Temperature2m { get; set; }
            [JsonPropertyName("weather_code")] public int[]? WeatherCode { get; set; }
            [JsonPropertyName("precipitation_probability")] public int[]? PrecipitationProbability { get; set; }
            [JsonPropertyName("wind_speed_10m")] public double[]? WindSpeed10m { get; set; }
        }

    }
}
