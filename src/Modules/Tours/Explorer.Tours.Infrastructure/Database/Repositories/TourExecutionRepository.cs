using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class TourExecutionRepository : ITourExecutionRepository
{
    private readonly ToursContext _dbContext;

    public TourExecutionRepository(ToursContext dbContext)
    {
        _dbContext = dbContext;
    }

    public TourExecution Create(TourExecution tourExecution)
    {
        _dbContext.TourExecutions.Add(tourExecution);
        _dbContext.SaveChanges();
        return tourExecution;
    }

    public TourExecution Update(TourExecution tourExecution)
    {
        _dbContext.TourExecutions.Update(tourExecution);
        _dbContext.SaveChanges();
        return tourExecution;
    }

    public TourExecution? Get(long id)
    {
        return _dbContext.TourExecutions.FirstOrDefault(te => te.Id == id);
    }

    public TourExecution? GetByTouristAndTour(int touristId, int tourId)
    {
        return _dbContext.TourExecutions
            .FirstOrDefault(te => te.IdTourist == touristId && te.IdTour == tourId);
    }

    public List<TourExecution> GetByTourist(int touristId)
    {
        return _dbContext.TourExecutions
            .Where(te => te.IdTourist == touristId)
            .ToList();
    }

    public List<TourExecution> GetByTour(int tourId)
    {
        return _dbContext.TourExecutions
            .Where(te => te.IdTour == tourId)
            .ToList();
    }

    public void Delete(long id)
    {
        var tourExecution = Get(id);
        if (tourExecution != null)
        {
            _dbContext.TourExecutions.Remove(tourExecution);
            _dbContext.SaveChanges();
        }
    }
}