using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    public class TransportTypePreferenceDto
    {
        //ovde sam stavio da bude string isto kao kod preferenci
        public string Transport { get; set; }
        public int Rating { get; set; }
    }
}
