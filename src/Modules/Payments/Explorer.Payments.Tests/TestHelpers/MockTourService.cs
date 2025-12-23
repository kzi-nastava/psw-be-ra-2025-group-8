using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Payments.Tests.TestHelpers
{
    public class MockTourService : ITourService
    {
        public TourDto Create(TourDto tour) => throw new NotImplementedException();
        public void Delete(long id, int authorId) => throw new NotImplementedException();
        public List<TourDto> GetByAuthor(int authorId) => new List<TourDto>();
        public PagedResult<TourDto> GetPaged(int page, int pageSize) => new PagedResult<TourDto>(new List<TourDto>(), 0);
        public TourDto GetById(long id)
        {
            var map = new Dictionary<long, decimal>
            {
                { -511, 50m },
                { -522, 100m },
                { -533, 70m }
            };

            if (!map.ContainsKey(id)) return null;

            return new TourDto
            {
                Id = (int)id,
                Name = "Mock tour",
                Description = "Mock",
                Difficulty = 1,
                Tags = new List<string>(),
                Status = "Published",
                Price = map[id],
                AuthorId = 1,
                KeyPoints = new List<KeyPointDto>(),
                LengthInKilometers = 0,
                RequiredEquipment = new List<TourEquipmentDto>(),
                PublishedAt = DateTime.UtcNow
            };
        }
        public TourDto Publish(long tourId, int authorId) => throw new NotImplementedException();
        public TourDto Reactivate(long tourId, int authorId) => throw new NotImplementedException();
        public TourDto Archive(long tourId, int authorId) => throw new NotImplementedException();
        public TourDto AddKeyPoint(long tourId, KeyPointDto keyPoint, int authorId) => throw new NotImplementedException();
        public TourDto AddEquipment(long tourId, long equipmentId, int authorId) => throw new NotImplementedException();
        public TourDto RemoveEquipment(long tourId, long equipmentId, int authorId) => throw new NotImplementedException();
        public TourDto AddTag(long tourId, string tag, int authorId) => throw new NotImplementedException();
        public TourDto RemoveTag(long tourId, string tag, int authorId) => throw new NotImplementedException();
        public TourDto Update(TourDto tour) => throw new NotImplementedException();
        public TourDto UpdateTags(long tourId, List<string> tags, int authorId) => throw new NotImplementedException();
        public TourDto UpdateTransportTimes(long tourId, List<TourTransportTimeDto> times, int authorId) => throw new NotImplementedException();
        public TourDto UpdateEquipment(long tourId, List<long> equipmentIds, int authorId) => throw new NotImplementedException();
        public List<EquipmentForTourDto> GetEquipmentForTour(long tourId, int authorId) => new List<EquipmentForTourDto>();
    }
}
