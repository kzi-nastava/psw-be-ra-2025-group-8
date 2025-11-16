using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class PreferencesDto
    {
        public long PersonId { get; set; }
        public string Difficulty { get; set; }
        public Dictionary<string, int> TransportRatings { get; set; }
        public List<string> Tags { get; set; }
    }
}
