using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITouristTourService
    {
        List<TouristTourPreviewDto> GetPublishedTours();
        TouristTourDetailsDto GetPublishedTourDetails(long id);
        List<KeyPointDto> GetTourKeyPoints(long tourId);
        List<TouristTourPreviewDto> SearchToursByLocation(TourSearchByLocationDto searchDto);
    }
}
