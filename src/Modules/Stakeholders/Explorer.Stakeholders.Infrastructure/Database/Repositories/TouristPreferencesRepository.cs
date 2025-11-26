using Explorer.Stakeholders.API.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class TouristPreferencesRepository : ITouristPreferencesRepository
    {
        private readonly StakeholdersContext _dbContext;
        private readonly DbSet<TouristPreferences> _dbSet;

        public TouristPreferencesRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TouristPreferences>();
        }

        public TouristPreferences GetByPersonId(long personId)
        {
            // Tražimo po PersonId, ne po primarnom ključu (Id), ovo mi resava problem od ranije error 500
            return _dbSet.Include(p => p.Person).FirstOrDefault(p => p.PersonId == personId);
        }

        public void DeleteByPersonId(long personId)
        {
            var touristPreferencesToDelete = GetByPersonId(personId);

            if (touristPreferencesToDelete == null)
            {
                throw new KeyNotFoundException($"TouristPreferences for PersonId {personId} not found.");
            }

            _dbSet.Remove(touristPreferencesToDelete);
            _dbContext.SaveChanges();
        }

        public TouristPreferences Create(TouristPreferences touristPreferences)
        {
            _dbSet.Add(touristPreferences);
            _dbContext.SaveChanges();


            //ovo ce da postavi 4 torke u TransportType cim se kreira preferenca (kod registracije novog korisnika)
            var transports = Enum.GetValues(typeof(TransportType))
                                .Cast<TransportType>()
                                .Select(t => new TransportTypePreferences(touristPreferences.Id, t, 0))
                                .ToList();


            _dbContext.Set<TransportTypePreferences>().AddRange(transports);
            _dbContext.SaveChanges();
            return touristPreferences;
        }

        public TouristPreferences Update(TouristPreferences touristPreferences)
        {
            _dbContext.Update(touristPreferences);
            _dbContext.SaveChanges();
            return touristPreferences;
        }
    }
}

