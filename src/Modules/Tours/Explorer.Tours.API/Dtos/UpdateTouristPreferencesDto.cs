using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class UpdateTouristPreferencesDto
    {
        public string Difficulty { get; set; }
        //ovaj poseban dto je ovde cisto da se ne prosledjuje ID u PUT zahtevu
    }
}

