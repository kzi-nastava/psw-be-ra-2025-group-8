using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourRatingImageService
    {
        TourRatingImageDto Create(TourRatingImageDto imageDto);
        List<TourRatingImageDto> GetByTourRatingId(long tourRatingId);
        TourRatingImageDto Get(long id);
        void Delete(long id);
    }
}
