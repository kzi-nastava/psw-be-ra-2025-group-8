using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TouristTourPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public double AverageRating { get; set; }

        public KeyPointPreviewDto FirstKeyPoint { get; set; }

        public List<string> Tags { get; set; }
        public List<string> RequiredEquipment { get; set; }
    }
}
