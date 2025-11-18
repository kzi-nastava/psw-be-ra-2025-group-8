using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;

using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class PreferenceTagsRepository : IPreferenceTagsRepository
    {
        private readonly StakeholdersContext _db;

        public PreferenceTagsRepository(StakeholdersContext db)
        {
            _db = db;
        }

        public IEnumerable<Tags> GetTagsForPerson(long personId)
        {
            // Join PreferenceTags -> TouristPreferences (PersonId) -> Tags
            var tags = from pt in _db.Set<PreferenceTags>()
                       join tp in _db.Set<TouristPreferences>() on pt.TouristPreferencesId equals tp.Id
                       join t in _db.Set<Tags>() on pt.TagsId equals t.Id
                       where tp.PersonId == personId
                       select t;

            return tags.Distinct().ToList();
        }

        public bool Exists(long touristPreferencesId, long tagsId)
        {
            return _db.Set<PreferenceTags>().Any(x => x.TouristPreferencesId == touristPreferencesId && x.TagsId == tagsId);
        }

        public PreferenceTags Add(PreferenceTags pt)
        {
            var e = _db.Set<PreferenceTags>().Add(pt).Entity;
            _db.SaveChanges();
            return e;
        }

        public void Delete(long touristPreferencesId, long tagsId)
        {
            var entry = _db.Set<PreferenceTags>().FirstOrDefault(x => x.TouristPreferencesId == touristPreferencesId && x.TagsId == tagsId);
            if (entry != null)
            {
                _db.Set<PreferenceTags>().Remove(entry);
                _db.SaveChanges();
            }
        }

    }
}
