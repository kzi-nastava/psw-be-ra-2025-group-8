using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;


namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TransportTypePreferencesRepository : ITransportTypePreferencesRepository
    {
        private readonly ToursContext _dbContext;


        public TransportTypePreferencesRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<TransportTypePreferences> GetByPreferenceId(long preferenceId)
        {
            return _dbContext.Set<TransportTypePreferences>().Where(x => x.PreferenceId == preferenceId).ToList();
        }


        public void CreateRange(IEnumerable<TransportTypePreferences> items)
        {
            _dbContext.Set<TransportTypePreferences>().AddRange(items);
            _dbContext.SaveChanges();
        }


        public void UpdateRange(IEnumerable<TransportTypePreferences> items)
        {
            _dbContext.Set<TransportTypePreferences>().UpdateRange(items);
            _dbContext.SaveChanges();
        }


        public void DeleteByPreferenceId(long preferenceId)
        {
            var toDelete = _dbContext.Set<TransportTypePreferences>().Where(x => x.PreferenceId == preferenceId).ToList();
            if (toDelete.Any())
            {
                _dbContext.Set<TransportTypePreferences>().RemoveRange(toDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
