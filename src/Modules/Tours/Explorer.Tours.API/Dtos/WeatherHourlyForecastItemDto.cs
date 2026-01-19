using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Explorer.Tours.API.Dtos;

public class WeatherHourlyForecastItemDto
{
    public DateTime Time { get; set; }
    public int WeatherCode { get; set; }
    public double TemperatureC { get; set; }

    public int? PrecipitationProbability { get; set; }
    public double? WindSpeedKmh { get; set; }
}

