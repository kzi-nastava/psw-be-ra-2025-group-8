using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IPositionRepository
{
    Position GetByTouristId(int touristId);
    Position Update(Position position);
    Position Create(Position position);
}
