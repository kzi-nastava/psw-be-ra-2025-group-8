using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourExecutionRepository
{
    TourExecution Create(TourExecution tourExecution);
    TourExecution Update(TourExecution tourExecution);
    TourExecution? Get(long id);
    TourExecution? GetByTouristAndTour(int touristId, int tourId);
    List<TourExecution> GetByTourist(int touristId);
    List<TourExecution> GetByTour(int tourId);
    void Delete(long id);
}