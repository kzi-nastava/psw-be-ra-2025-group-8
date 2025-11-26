using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IPreferenceTagsRepository
    {
        IEnumerable<Tags> GetTagsForPerson(long personId);
        bool Exists(long touristPreferencesId, long tagsId);
        PreferenceTags Add(PreferenceTags pt);
        void Delete(long touristPreferencesId, long tagsId);
    }
}
