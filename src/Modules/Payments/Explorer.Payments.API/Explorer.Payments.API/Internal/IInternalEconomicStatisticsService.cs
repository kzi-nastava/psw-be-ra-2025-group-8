using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Internal
{
    public interface IInternalEconomicStatisticsService
    {
        AuthorRevenueStatsDto GetRevenueStatsForTours(IEnumerable<long> tourIds);
    }
}
