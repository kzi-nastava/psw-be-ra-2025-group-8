using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IKeyPointReachedRepository
{
    KeyPointReached Create(KeyPointReached keyPointReached);
    List<KeyPointReached> GetByTourExecution(long tourExecutionId);
    List<int> GetReachedKeyPointOrders(long tourExecutionId);
}
