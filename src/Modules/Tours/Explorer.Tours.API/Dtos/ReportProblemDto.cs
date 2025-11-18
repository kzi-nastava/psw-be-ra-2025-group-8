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
    }
}
