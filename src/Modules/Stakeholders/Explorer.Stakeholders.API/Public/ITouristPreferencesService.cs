using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface ITouristPreferencesService
    {
        TouristPreferencesDto Get(long personId);
        TouristPreferencesDto Update(long personId, UpdateTouristPreferencesDto dto);
    }
}

