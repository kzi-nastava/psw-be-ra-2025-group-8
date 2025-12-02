using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class ReportProblemDto
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public int TouristId { get; set; }
        public int Category { get; set; }
        public int Priority { get; set; }
        public string Description { get; set; }
        public DateTime ReportTime { get; set; }

        // Author response
        public int? AuthorId { get; set; }
        public string? AuthorResponse { get; set; }
        public DateTime? AuthorResponseTime { get; set; }

        // Tourist resolution
        public bool? IsResolved { get; set; }
        public string? TouristResolutionComment { get; set; }
        public DateTime? TouristResolutionTime { get; set; }
    }
}
