using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TouristTourDetailsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public decimal Price { get; set; }
        public string Author { get; set; }
        public double LengthInKilometers { get; set; }

        public KeyPointPreviewDto FirstKeyPoint { get; set; }

        public List<string> Tags { get; set; }
        public List<string> RequiredEquipment { get; set; }

        public double AverageRating { get; set; }
        public List<TourReviewDto> Reviews { get; set; }
    }
}
