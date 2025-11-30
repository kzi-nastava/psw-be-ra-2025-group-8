using Explorer.Stakeholders.API.Public;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TouristPreferencesRepository : ITouristPreferencesRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TouristPreferences> _dbSet;

        public TouristPreferencesRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TouristPreferences>();
        }

        public TouristPreferences GetByPersonId(long personId)
        {
            return _dbSet
                .Include(tp => tp.TransportTypePreferences)
                .Include(tp => tp.PreferenceTags)
                    .ThenInclude(pt => pt.Tags)
                .FirstOrDefault(tp => tp.PersonId == personId);
        }

        public TouristPreferences Create(TouristPreferences touristPreferences)
        {
            _dbSet.Add(touristPreferences);
            _dbContext.SaveChanges();
            return touristPreferences;
        }

        public TouristPreferences Update(TouristPreferences touristPreferences)
        {
            _dbSet.Update(touristPreferences);
            _dbContext.SaveChanges();
            return touristPreferences;
        }

        public void DeleteByPersonId(long personId)
        {
            var touristPreferences = GetByPersonId(personId);
            if (touristPreferences == null) return;

            _dbSet.Remove(touristPreferences);
            _dbContext.SaveChanges();
        }
    }
}

