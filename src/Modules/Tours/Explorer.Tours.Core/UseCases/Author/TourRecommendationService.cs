using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Author;

public class TourRecommendationService : ITourRecommendationService
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly ITourStatsRepository _tourStatsRepository;

    // Thresholds for recommendation logic
    private const double HighThreshold = 85.0;
    private const double LowThreshold = 30.0;
    private const int LongDurationMinutes = 6 * 60; // 6 hours in minutes

    public TourRecommendationService(
        ITourRepository tourRepository,
        ITourExecutionRepository tourExecutionRepository,
        ITourStatsRepository tourStatsRepository)
    {
        _tourRepository = tourRepository;
        _tourExecutionRepository = tourExecutionRepository;
        _tourStatsRepository = tourStatsRepository;
    }

    public AuthorRecommendationsDto GetRecommendationsForAuthor(int authorId)
    {
        var recommendations = new List<TourRecommendationDto>();
        var authorTours = _tourRepository.GetByAuthor(authorId);

        foreach (var tour in authorTours)
        {
            var tourRecommendations = GenerateRecommendationsForTour(tour);
            recommendations.AddRange(tourRecommendations);
        }

        recommendations.AddRange(AnalyzeBundlingOpportunities(authorTours));

        return new AuthorRecommendationsDto
        {
            AuthorId = authorId,
            Recommendations = recommendations,
            GeneratedAt = DateTime.UtcNow
        };
    }

    private List<TourRecommendationDto> GenerateRecommendationsForTour(Tour tour)
    {
        var recommendations = new List<TourRecommendationDto>();
        var executions = _tourExecutionRepository.GetByTour((int)tour.Id);

        // Need at least some executions to generate meaningful recommendations
        if (executions.Count < 3)
        {
            return recommendations;
        }

        var stats = CalculateStatsForTour(executions);
        var expectedDurationMinutes = GetExpectedDurationMinutes(tour);

        // Generate recommendations based on various metrics combinations
        recommendations.AddRange(AnalyzeCompletionMetrics(tour, stats));
        recommendations.AddRange(AnalyzeDurationMetrics(tour, stats, expectedDurationMinutes));
        recommendations.AddRange(AnalyzePositiveMetrics(tour, stats));
        recommendations.AddRange(AnalyzeEconomicMetrics(tour));

        return recommendations;
    }

    private TourStatsInfo CalculateStatsForTour(List<TourExecution> executions)
    {
        var finishedExecutions = executions
            .Where(te => te.Status == TourExecution.TourExecutionStatus.Completed ||
                         te.Status == TourExecution.TourExecutionStatus.Abandoned)
            .ToList();

        double completionRate = 0;
        double avgCompletionPercentage = 0;

        if (finishedExecutions.Any())
        {
            var completedCount = finishedExecutions.Count(te => te.Status == TourExecution.TourExecutionStatus.Completed);
            completionRate = (double)completedCount / finishedExecutions.Count * 100;
            avgCompletionPercentage = finishedExecutions.Average(te => te.CompletionPercentage);
        }

        // Calculate average duration (excluding executions > 12 hours)
        const double MaxDurationMinutes = 12 * 60;
        var validDurationExecutions = executions
            .Where(te =>
            {
                var durationMinutes = te.LastActivity.Subtract(te.CreatedAt).TotalMinutes;
                return durationMinutes > 1 && durationMinutes <= MaxDurationMinutes;
            })
            .ToList();

        double avgDurationMinutes = validDurationExecutions.Any()
            ? validDurationExecutions.Average(te => te.LastActivity.Subtract(te.CreatedAt).TotalMinutes)
            : 0;

        return new TourStatsInfo
        {
            CompletionRate = completionRate,
            AverageCompletionPercentage = avgCompletionPercentage,
            AverageDurationMinutes = avgDurationMinutes,
            TotalExecutions = executions.Count,
            FinishedExecutions = finishedExecutions.Count
        };
    }

    private int GetExpectedDurationMinutes(Tour tour)
    {
        if (tour.TransportTimes == null || !tour.TransportTimes.Any())
        {
            return 0;
        }

        // Get the longest transport time as the expected duration
        return tour.TransportTimes.Max(t => t.DurationMinutes);
    }

    private List<TourRecommendationDto> AnalyzeCompletionMetrics(Tour tour, TourStatsInfo stats)
    {
        var recommendations = new List<TourRecommendationDto>();

        // High completion percentage but low completion rate
        // People do most of the tour but don't finish
        if (stats.AverageCompletionPercentage >= HighThreshold && stats.CompletionRate < LowThreshold)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Turisti prolaze većinu ture ali je retko završavaju do kraja. Razmislite o tome da učinite poslednje tačke interesantnijim ili da skratite završni deo ture.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.General
            });
        }

        // Both metrics are low - tour might be too long or difficult
        if (stats.AverageCompletionPercentage < LowThreshold && stats.CompletionRate < LowThreshold)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Turisti završavaju mali deo ture i retko je dovršavaju. Tura je možda predugačka ili preteška. Razmislite o skraćivanju ture ili prilagođavanju težine.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.General
            });
        }

        // Low completion percentage but decent completion rate
        // People who start don't get far, but those who do finish
        if (stats.AverageCompletionPercentage < LowThreshold && stats.CompletionRate >= 50)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Prosečna procentualna završenost ture je niska. Razmislite o dodavanju zanimljivih sadržaja na početnim tačkama ture kako biste zadržali turiste.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.General
            });
        }

        return recommendations;
    }

    private List<TourRecommendationDto> AnalyzeDurationMetrics(Tour tour, TourStatsInfo stats, int expectedDurationMinutes)
    {
        var recommendations = new List<TourRecommendationDto>();

        // Tour takes longer than 6 hours on average
        if (stats.AverageDurationMinutes > LongDurationMinutes)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = $"Prosečno vreme izvršavanja ture je preko 6 sati ({FormatDuration(stats.AverageDurationMinutes)}). Razmislite o podeli ture na više kraćih tura.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.General
            });
        }

        // Tour takes significantly longer than expected (author's estimate)
        if (expectedDurationMinutes > 0 && stats.AverageDurationMinutes > expectedDurationMinutes * 1.5)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = $"Tura u proseku traje {FormatDuration(stats.AverageDurationMinutes)}, što je značajno duže od vašeg predviđenog vremena ({FormatDuration(expectedDurationMinutes)}). Razmislite o ažuriranju procenjenog vremena ili prilagođavanju ture.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.General
            });
        }

        // Long duration combined with low completion percentage
        if (stats.AverageDurationMinutes > LongDurationMinutes && stats.AverageCompletionPercentage < LowThreshold)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Tura je predugačka - turisti provode mnogo vremena ali prolaze mali procenat. Preporučujemo značajno skraćivanje ture.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.General
            });
        }

        return recommendations;
    }

    private List<TourRecommendationDto> AnalyzePositiveMetrics(Tour tour, TourStatsInfo stats)
    {
        var recommendations = new List<TourRecommendationDto>();

        // Excellent completion rate - tour is very successful
        if (stats.CompletionRate >= HighThreshold && stats.AverageCompletionPercentage >= HighThreshold)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Ova tura ima odličan procenat završenosti! Razmislite o kreiranju slične ture ili dodavanju dodatnih tačaka kako biste je proširili.",
                Sentiment = RecommendationSentiment.Positive,
                Category = RecommendationCategory.General
            });
        }

        // High completion rate
        if (stats.CompletionRate >= HighThreshold && stats.CompletionRate < 95)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Veliki procenat turista završava ovu turu. Možete razmisliti o produženju ture dodavanjem novih interesantnih tačaka.",
                Sentiment = RecommendationSentiment.Positive,
                Category = RecommendationCategory.General
            });
        }

        // Tour is completed quickly with high success
        if (stats.CompletionRate >= HighThreshold && stats.AverageDurationMinutes > 0 && stats.AverageDurationMinutes < 120)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Tura se brzo završava sa visokim procentom uspešnosti. Idealna je za turiste sa ograničenim vremenom. Razmislite o kreiranju sličnih kratkih tura.",
                Sentiment = RecommendationSentiment.Positive,
                Category = RecommendationCategory.General
            });
        }

        return recommendations;
    }

    private List<TourRecommendationDto> AnalyzeEconomicMetrics(Tour tour)
    {
        var recommendations = new List<TourRecommendationDto>();
        var stats = _tourStatsRepository.GetByTourId((int)tour.Id);

        if (stats == null) return recommendations;

        // Cold start
        if (stats.TotalSales < 2 && tour.PublishedAt < DateTime.UtcNow.AddMonths(-1))
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Ova tura ima veoma malo prodaja od objavljivanja 'Cold Start'. Razmislite o promotivnoj ceni ili popustu.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.Economic
            });
        }
        // Bestseller
        if (stats.TotalSales > 30)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Ova tura je bestseler! Razmislite o tome da napravite 'Premium' verziju ili nastavak ove rute.",
                Sentiment = RecommendationSentiment.Positive,
                Category = RecommendationCategory.Economic
            });
        }
        // Money maker
        if (stats.TotalRevenue > 50000)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Ova tura je 'Money Maker'. Generiše značajan prihod. Razmislite o dodatnom marketingu za ovaj segment.",
                Sentiment = RecommendationSentiment.Positive,
                Category = RecommendationCategory.Economic
            });
        }
        // Discount King
        if (stats.TotalSales > 5) // Potreban bar mali uzorak podataka
        {
            decimal averagePricePaid = (decimal)(stats.TotalRevenue / stats.TotalSales);
            if (averagePricePaid < (tour.Price * 0.7m)) // Ako je prosečna cena 30% niža od originalne
            {
                recommendations.Add(new TourRecommendationDto
                {
                    TourId = (int)tour.Id,
                    TourName = tour.Name,
                    Message = "'Discount King' Većina prodaja ove ture ostvarena je putem kupona ili popusta. Razmislite o trajnom sniženju osnovne cene.",
                    Sentiment = RecommendationSentiment.Positive,
                    Category = RecommendationCategory.Economic
                });
            }
        }
        // Falloff
        if (stats.LastPurchaseDate.HasValue && stats.LastPurchaseDate < DateTime.UtcNow.AddMonths(-3) && stats.TotalSales > 0)
        {
            recommendations.Add(new TourRecommendationDto
            {
                TourId = (int)tour.Id,
                TourName = tour.Name,
                Message = "Interesovanje za ovu turu opada 'Falloff'. Poslednja prodaja je bila pre više od 3 meseca.",
                Sentiment = RecommendationSentiment.Negative,
                Category = RecommendationCategory.Economic
            });
        }

        return recommendations;
    }

    private List<TourRecommendationDto> AnalyzeBundlingOpportunities(List<Tour> allAuthorTours)
    {
        var recommendations = new List<TourRecommendationDto>();

        var successfulTours = allAuthorTours
            .Where(t => t.Status == TourStatus.Published)
            .Where(t => {
                var s = _tourStatsRepository.GetByTourId((int)t.Id);
                return s != null && s.TotalSales > 5;
            }).ToList();

        var groupsByDifficulty = successfulTours.GroupBy(t => t.Difficulty);

        foreach (var group in groupsByDifficulty)
        {
            if (group.Count() >= 2)
            {
                var tourNames = group.Select(t => t.Name).Take(2).ToList();
                var firstTour = group.First();
                string difficultyName = GetDifficultyName(group.Key);

                recommendations.Add(new TourRecommendationDto
                {
                    TourId = (int)firstTour.Id,
                    TourName = firstTour.Name,
                    Message = $"Primetili smo da imate više uspešnih tura težine '{difficultyName}' " +
                              $"(npr. '{tourNames[0]}' i '{tourNames[1]}'). " +
                              "Kreirajte paket za ovu ciljnu grupu uz mali popust!",
                    Sentiment = RecommendationSentiment.Positive,
                    Category = RecommendationCategory.Economic
                });
            }
        }

        return recommendations;
    }

    private string GetDifficultyName(int difficulty) => difficulty switch
    {
        1 => "Easy",
        2 => "Medium",
        3 => "Hard",
        _ => "Unknown"
    };

    private string FormatDuration(double minutes)
    {
        var timeSpan = TimeSpan.FromMinutes(minutes);
        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}min";
        }
        return $"{(int)minutes}min";
    }

    private class TourStatsInfo
    {
        public double CompletionRate { get; set; }
        public double AverageCompletionPercentage { get; set; }
        public double AverageDurationMinutes { get; set; }
        public int TotalExecutions { get; set; }
        public int FinishedExecutions { get; set; }
    }
}
