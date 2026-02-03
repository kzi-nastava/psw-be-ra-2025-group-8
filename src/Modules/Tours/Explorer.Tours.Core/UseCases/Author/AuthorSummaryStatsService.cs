using Explorer.Payments.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Author;

public class AuthorSummaryStatsService : IAuthorSummaryStatsService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourRatingRepository _tourRatingRepository;
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly IInternalEconomicStatisticsService _economicStatisticsService;

    public AuthorSummaryStatsService(
        ITourRepository tourRepository,
        ITourRatingRepository tourRatingRepository,
        ITourExecutionRepository tourExecutionRepository,
        IInternalEconomicStatisticsService economicStatisticsService)
    {
        _tourRepository = tourRepository;
        _tourRatingRepository = tourRatingRepository;
        _tourExecutionRepository = tourExecutionRepository;
        _economicStatisticsService = economicStatisticsService;
    }

    public AuthorQuickStatsDto GetQuickStats(int authorId)
    {
        var authorTours = _tourRepository.GetByAuthor(authorId);
        var tourIds = authorTours.Select(t => t.Id).ToList();

        // Get revenue stats from Payments module
        var revenueStats = _economicStatisticsService.GetRevenueStatsForTours(tourIds);

        // Calculate ratings
        var allRatings = new List<TourRating>();
        foreach (var tourId in tourIds)
        {
            var ratings = _tourRatingRepository.GetByTour((int)tourId);
            allRatings.AddRange(ratings);
        }
        double averageRating = allRatings.Any() ? allRatings.Average(r => r.Rating) : 0;

        // Calculate executions and completions
        var allExecutions = new List<TourExecution>();
        foreach (var tourId in tourIds)
        {
            var executions = _tourExecutionRepository.GetByTour((int)tourId);
            allExecutions.AddRange(executions);
        }

        var uniqueTourists = allExecutions.Select(e => e.IdTourist).Distinct().Count();
        var totalCompletions = allExecutions.Count(e => e.Status == TourExecution.TourExecutionStatus.Completed);

        // Find most popular tour
        MostPopularTourDto? mostPopularTour = null;
        if (authorTours.Any() && revenueStats.RevenueByTour.Any())
        {
            var mostPurchasedTourId = revenueStats.RevenueByTour
                .OrderByDescending(kvp => kvp.Value.PurchaseCount)
                .First().Key;

            var tour = authorTours.FirstOrDefault(t => t.Id == mostPurchasedTourId);
            if (tour != null)
            {
                var tourRatings = _tourRatingRepository.GetByTour((int)tour.Id);
                var tourCompletions = _tourExecutionRepository.GetByTour((int)tour.Id)
                    .Count(e => e.Status == TourExecution.TourExecutionStatus.Completed);

                mostPopularTour = new MostPopularTourDto
                {
                    TourId = (int)tour.Id,
                    TourName = tour.Name,
                    PurchaseCount = revenueStats.RevenueByTour[tour.Id].PurchaseCount,
                    CompletionCount = tourCompletions,
                    AverageRating = tourRatings.Any() ? tourRatings.Average(r => r.Rating) : 0
                };
            }
        }

        return new AuthorQuickStatsDto
        {
            TotalTours = authorTours.Count,
            PublishedTours = authorTours.Count(t => t.Status == TourStatus.Published),
            TotalRevenue = revenueStats.TotalRevenue,
            AverageRating = Math.Round(averageRating, 2),
            TotalTourists = uniqueTourists,
            TotalCompletions = totalCompletions,
            MostPopularTour = mostPopularTour
        };
    }

    public AuthorTourTrendsDto GetRatingTrends(int authorId)
    {
        var authorTours = _tourRepository.GetByAuthor(authorId);
        var trends = new List<TourRatingTrendDto>();

        var now = DateTime.UtcNow;
        var currentMonthStart = new DateTime(now.Year, now.Month, 1);
        var previousMonthStart = currentMonthStart.AddMonths(-1);
        var previousMonthEnd = currentMonthStart.AddDays(-1);

        foreach (var tour in authorTours)
        {
            var ratings = _tourRatingRepository.GetByTour((int)tour.Id);

            var currentMonthRatings = ratings
                .Where(r => r.CreatedAt >= currentMonthStart)
                .ToList();

            var previousMonthRatings = ratings
                .Where(r => r.CreatedAt >= previousMonthStart && r.CreatedAt <= previousMonthEnd)
                .ToList();

            var currentAvg = currentMonthRatings.Any() ? currentMonthRatings.Average(r => r.Rating) : 0;
            var previousAvg = previousMonthRatings.Any() ? previousMonthRatings.Average(r => r.Rating) : 0;

            TrendDirection trend;
            double percentageChange = 0;

            if (!currentMonthRatings.Any() && !previousMonthRatings.Any())
            {
                trend = TrendDirection.NoData;
            }
            else if (!previousMonthRatings.Any())
            {
                trend = currentMonthRatings.Any() ? TrendDirection.Up : TrendDirection.NoData;
            }
            else if (!currentMonthRatings.Any())
            {
                trend = TrendDirection.Down;
                percentageChange = -100;
            }
            else
            {
                percentageChange = previousAvg > 0 ? ((currentAvg - previousAvg) / previousAvg) * 100 : 0;

                if (Math.Abs(percentageChange) < 5)
                {
                    trend = TrendDirection.Stable;
                }
                else if (currentAvg > previousAvg)
                {
                    trend = TrendDirection.Up;
                }
                else
                {
                    trend = TrendDirection.Down;
                }
            }

            trends.Add(new TourRatingTrendDto
            {
                TourId = (int)tour.Id,
                CurrentMonthRating = Math.Round(currentAvg, 2),
                PreviousMonthRating = Math.Round(previousAvg, 2),
                Trend = trend,
                PercentageChange = Math.Round(percentageChange, 1),
                CurrentMonthReviewCount = currentMonthRatings.Count,
                PreviousMonthReviewCount = previousMonthRatings.Count
            });
        }

        return new AuthorTourTrendsDto
        {
            AuthorId = authorId,
            TourTrends = trends,
            GeneratedAt = DateTime.UtcNow
        };
    }
}
