using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface IEconomicStatisticsService
    {
        List<TourEconomicStatisticsDto> GetStatisticsForTour(long tourId, StatisticsInterval interval, int offset);
    }
}