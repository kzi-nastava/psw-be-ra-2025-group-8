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
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class PreferenceTagsRepository : IPreferenceTagsRepository
    {
        private readonly ToursContext _db;

        public PreferenceTagsRepository(ToursContext db)
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
                //Maksim: dodao try-catch za svaki slucaj, jer ponekad zbog redosleda izvrsavanja testova jedan test padne jer pokusava istu stvar da uradi opet
                try
                {
                    _db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Another test/operation removed the same link concurrently; treat as success
                }
            }
        }

    }
}
