using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourStatsRepository
{
    TourStats? GetByTourId(int tourId);
    TourStats Create(TourStats tourStats);
    TourStats Update(TourStats tourStats);
}
