using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourRatingRepository : ITourRatingRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TourRating> _dbSet;

        public TourRatingRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TourRating>();
        }

        public PagedResult<TourRating> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public TourRating Create(TourRating tourRating)
        {
            _dbContext.TourRatings.Add(tourRating);
            _dbContext.SaveChanges();
            return tourRating;
        }

        public TourRating Update(TourRating tourRating)
        {
            _dbContext.TourRatings.Update(tourRating);
            _dbContext.SaveChanges();
            return tourRating;
        }

        public TourRating? Get(long id)
        {
            return _dbContext.TourRatings.FirstOrDefault(te => te.Id == id);
        }

        public TourRating? GetByTouristAndTour(int touristId, int tourId)
        {
            return _dbContext.TourRatings
                .FirstOrDefault(te => te.IdTourist == touristId && te.IdTour == tourId);
        }

        public List<TourRating> GetByTourist(int touristId)
        {
            return _dbContext.TourRatings
                .Where(te => te.IdTourist == touristId)
                .ToList();
        }

        public List<TourRating> GetByTour(int tourId)
        {
            return _dbContext.TourRatings
                .Where(te => te.IdTour == tourId)
                .ToList();
        }

        public void Delete(long id)
        {
            var tourRating = Get(id);
            if (tourRating != null)
            {
                _dbContext.TourRatings.Remove(tourRating);
                _dbContext.SaveChanges();
            }
        }
    }
}
