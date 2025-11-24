using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repo;

        public PositionService(IPositionRepository repo)
        {
            _repo = repo;
        }

        public PositionDto GetByTouristId(int touristId)
        {
            var pos = _repo.GetByTouristId(touristId);
            return new PositionDto
            {
                Id = (int)pos.Id,
                Latitude = pos.Latitude,
                Longitude = pos.Longitude,
                TouristId = pos.TouristId,
                UpdatedAt = pos.UpdatedAt
            };
        }

        public PositionDto CreatePosition(PositionDto dto)
        {
            var pos = new Position(dto.Latitude, dto.Longitude, dto.TouristId);
            var created = _repo.Create(pos);

            return new PositionDto
            {
                Id = (int)created.Id,
                Latitude = created.Latitude,
                Longitude = created.Longitude,
                TouristId = created.TouristId,
                UpdatedAt = created.UpdatedAt
            };
        }

        public PositionDto UpdatePosition(PositionDto dto)
        {
            var pos = _repo.GetByTouristId(dto.TouristId);
            pos.UpdatePosition(dto.Latitude, dto.Longitude);

            var updated = _repo.Update(pos);

            return new PositionDto
            {
                Id = (int)updated.Id,
                Latitude = updated.Latitude,
                Longitude = updated.Longitude,
                TouristId = updated.TouristId,
                UpdatedAt = updated.UpdatedAt
            };
        }
    }
}
