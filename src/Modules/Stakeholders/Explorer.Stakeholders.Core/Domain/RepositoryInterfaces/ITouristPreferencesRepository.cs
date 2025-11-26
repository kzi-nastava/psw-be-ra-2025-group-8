using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface ITouristPreferencesRepository
    {
        TouristPreferences GetByPersonId(long personId);
        TouristPreferences Create(TouristPreferences touristPreferences);
        TouristPreferences Update(TouristPreferences touristPreferences);
        void DeleteByPersonId(long personId);
    }
}

