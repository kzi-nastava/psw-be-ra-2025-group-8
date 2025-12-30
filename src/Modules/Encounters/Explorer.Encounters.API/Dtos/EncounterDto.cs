using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Dtos
{
    public class EncounterDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Latitude { get;  set; }
        public double Longitude { get;  set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int XPReward { get; set; }
        public DateTime? PublishedAt { get;  set; }
        public DateTime? ArchivedAt { get;  set; }

        // Social encounter settings
        public int? SocialRequiredCount { get; set; }
        public double? SocialRangeMeters { get; set; }
    }
}
