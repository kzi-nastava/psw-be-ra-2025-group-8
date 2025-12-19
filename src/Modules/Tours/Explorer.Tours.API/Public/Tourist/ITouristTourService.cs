using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITouristTourService
    {
        List<TouristTourPreviewDto> GetPublishedTours();
        List<TouristTourPreviewDto> GetPublishedTours(int? minPrice, int? maxPrice);

        List<TouristTourPreviewDto> GetPublishedTours(
                long personId,
                bool searchByOwnedEquipment,
                bool searchByPreferenceTags,
                bool searchByPreferenceDifficulty,
                List<int>? difficulties,
                int? minPrice,
                int? maxPrice);

        TouristTourDetailsDto GetPublishedTourDetails(long id);
        List<TouristTourPreviewDto> GetPublishedTours(List<int> difficulties, int? minPrice, int? maxPrice);
        List<KeyPointDto> GetTourKeyPoints(long tourId);
        List<TouristTourPreviewDto> SearchToursByLocation(TourSearchByLocationDto searchDto);
    }
}
