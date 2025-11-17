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
        TouristPreferencesDto Create(long personId, TouristPreferencesDto dto);
        TouristPreferencesDto Update(long personId, TouristPreferencesDto dto);
        void Delete(long personId);
    }
}

