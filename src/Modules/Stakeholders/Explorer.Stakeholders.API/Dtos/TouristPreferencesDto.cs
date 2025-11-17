using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TouristPreferencesDto
    {
        public long PersonId { get; set; }
        public string Difficulty { get; set; }
        public List<TransportTypePreferenceDto> TransportPreferences { get; set; } = new List<TransportTypePreferenceDto>();
    }
}

