using Explorer.Stakeholders.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Public
{
    public interface IPreferencesService
    {
        PreferencesDto Get(long personId);
        PreferencesDto Create(long personId, PreferencesDto dto);
        PreferencesDto Update(long personId, PreferencesDto dto);
        void Delete(long personId);
    }
}
