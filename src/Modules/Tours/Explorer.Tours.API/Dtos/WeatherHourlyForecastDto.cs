using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Explorer.Tours.API.Dtos;

public class WeatherHourlyForecastDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Timezone { get; set; }

    // korisno za UI “Next keypoint: …”
    public int? KeyPointOrder { get; set; }
    public string? KeyPointName { get; set; }

    public List<WeatherHourlyForecastItemDto> Hours { get; set; } = new();
}

