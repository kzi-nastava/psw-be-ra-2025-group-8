using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Author;

public interface ITourStatsService
{
    TourStatsDto GetTourStats(int tourId);
}
