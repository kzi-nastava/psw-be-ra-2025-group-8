using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IKeyPointRepository
{
    List<KeyPoint> GetByTour(long tourId);
    KeyPoint Create(KeyPoint keyPoint);
    KeyPoint GetByTourAndOrder(long tourId, int order);
}
