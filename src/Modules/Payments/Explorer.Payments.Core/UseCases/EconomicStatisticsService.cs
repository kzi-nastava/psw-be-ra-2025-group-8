using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class EconomicStatisticsService : IEconomicStatisticsService
    {
        private readonly IPurchasedItemRepository _purchasedItemRepository;

        public EconomicStatisticsService(IPurchasedItemRepository purchasedItemRepository)
        {
            _purchasedItemRepository = purchasedItemRepository;
        }

        public List<TourEconomicStatisticsDto> GetStatisticsForTour(long tourId, StatisticsInterval interval, int offset = 0)
        {
            var purchases = _purchasedItemRepository.GetAll()
                .Where(pi => pi.TourId == tourId)
                .ToList();

            int steps = interval switch
            {
                StatisticsInterval.Weekly => 13,
                StatisticsInterval.Monthly => 12,
                StatisticsInterval.Yearly => 10,
                _ => 12
            };

            var result = new List<TourEconomicStatisticsDto>();
            DateTime referenceDate = interval switch
            {
                StatisticsInterval.Weekly => DateTime.Now.Date.AddDays(-offset * 7),
                StatisticsInterval.Monthly => DateTime.Now.Date.AddMonths(-offset),
                StatisticsInterval.Yearly => DateTime.Now.Date.AddYears(-offset),
                _ => DateTime.Now.Date
            };

            for (int i = steps - 1; i >= 0; i--)
            {
                DateTime periodStart = interval switch
                {
                    StatisticsInterval.Weekly => GetStartOfInterval(referenceDate.AddDays(-i * 7), interval),
                    StatisticsInterval.Monthly => GetStartOfInterval(referenceDate.AddMonths(-i), interval),
                    StatisticsInterval.Yearly => GetStartOfInterval(referenceDate.AddYears(-i), interval),
                    _ => referenceDate
                };

                var periodPurchases = purchases.Where(pi => {
                    DateTime piStart = GetStartOfInterval(pi.PurchaseDate, interval);
                    if (interval == StatisticsInterval.Yearly)
                        return piStart.Year == periodStart.Year;
                    if (interval == StatisticsInterval.Monthly)
                        return piStart.Year == periodStart.Year && piStart.Month == periodStart.Month;

                    return piStart.Date == periodStart.Date;
                }).ToList();

                result.Add(new TourEconomicStatisticsDto
                {
                    TimePeriod = periodStart,
                    TotalRevenue = periodPurchases.Sum(pi => pi.Price),
                    SalesCount = periodPurchases.Count(),
                    RevenueFromCoupons = periodPurchases.Where(pi => pi.CouponId.HasValue).Sum(pi => pi.Price),
                    RevenueFromSales = periodPurchases.Where(pi => pi.SaleId.HasValue).Sum(pi => pi.Price),
                    RevenueFullPrice = periodPurchases.Where(pi => !pi.CouponId.HasValue && !pi.SaleId.HasValue).Sum(pi => pi.Price)
                });
            }

            return result;
        }

        private DateTime GetStartOfInterval(DateTime date, StatisticsInterval interval)
        {
            switch (interval)
            {
                case StatisticsInterval.Weekly:
                    int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
                    return date.AddDays(-1 * diff).Date;
                case StatisticsInterval.Monthly:
                    return new DateTime(date.Year, date.Month, 1);
                case StatisticsInterval.Yearly:
                    return new DateTime(date.Year, 1, 1);
                default:
                    return date.Date;
            }
        }

        private bool IsAuthorOwner(long tourId, long authorId)
        {
            // Ovde bi idealno bilo pozvati interni servis iz Tours modula
            // ali za potrebe Sprinta 4, ako su podaci u istoj bazi, može i provera u repozitorijumu.
            return true; // Privremeno true dok ne povežemo provere vlasništva
        }
    }
}