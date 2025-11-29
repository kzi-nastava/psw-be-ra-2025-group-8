using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author
{
    public interface ITourService
    {
        PagedResult<TourDto> GetPaged(int page, int pageSize);
        TourDto Create(TourDto tour);
        TourDto Update(TourDto tour);
        List<TourDto> GetByAuthor(int authorId);
        void Delete(long id, int authorId);

        TourDto AddKeyPoint(long tourId, KeyPointDto keyPoint, int authorId);
        TourDto Publish(long tourId, int authorId);
        TourDto Archive(long tourId, int authorId);
    }
}