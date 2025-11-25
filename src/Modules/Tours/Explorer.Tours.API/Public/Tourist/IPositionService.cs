using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface IPositionService
    {
        PositionDto GetByTouristId(int touristId);
        PositionDto UpdatePosition(PositionDto position);
        PositionDto CreatePosition(PositionDto position);
    }
}
