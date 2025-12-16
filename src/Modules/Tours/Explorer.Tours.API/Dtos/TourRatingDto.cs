using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class TourRatingDto
    {
        public long Id { get; set; }
        public int IdTour { get; set; }
        public int IdTourist { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public double TourCompletionPercentage { get; set; } = 0;
    }
}
