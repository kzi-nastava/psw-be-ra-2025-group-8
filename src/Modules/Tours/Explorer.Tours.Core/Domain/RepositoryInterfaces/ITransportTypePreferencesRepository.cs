using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface ITransportTypePreferencesRepository
    {
        IEnumerable<TransportTypePreferences> GetByPreferenceId(long preferenceId);
        void CreateRange(IEnumerable<TransportTypePreferences> items);
        void UpdateRange(IEnumerable<TransportTypePreferences> items);
        void DeleteByPreferenceId(long preferenceId);
    }
}
