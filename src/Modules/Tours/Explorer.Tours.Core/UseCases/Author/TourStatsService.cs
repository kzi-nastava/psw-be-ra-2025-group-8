using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using static Explorer.Tours.Core.Domain.TourExecution;

namespace Explorer.Tours.Core.UseCases.Author;

public class TourStatsService : ITourStatsService
{
    private readonly ITourStatsRepository _tourStatsRepository;
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly IMapper _mapper;

    public TourStatsService(
        ITourStatsRepository tourStatsRepository,
        ITourExecutionRepository tourExecutionRepository,
        IMapper mapper)
    {
        _tourStatsRepository = tourStatsRepository;
        _tourExecutionRepository = tourExecutionRepository;
        _mapper = mapper;
    }

    public TourStatsDto GetTourStats(int tourId)
    {
        var existingStats = _tourStatsRepository.GetByTourId(tourId);

        // Check if recalculation is needed (if stats don't exist or are older than 24 hours)
        if (existingStats == null || existingStats.IsStale())
        {
            var calculatedStats = CalculateTourStats(tourId);

            if (existingStats == null)
            {
                // Create new stats
                existingStats = _tourStatsRepository.Create(calculatedStats);
            }
            else
            {
                // Update existing stats
                existingStats.Update(calculatedStats.CompletionRate, calculatedStats.AverageCompletionPercentage);
                existingStats = _tourStatsRepository.Update(existingStats);
            }
        }

        return _mapper.Map<TourStatsDto>(existingStats);
    }

    private TourStats CalculateTourStats(int tourId)
    {
        var allExecutions = _tourExecutionRepository.GetByTour(tourId);

        // Filter only Completed and Abandoned (exclude InProgress)
        var finishedExecutions = allExecutions
            .Where(te => te.Status == TourExecutionStatus.Completed ||
                         te.Status == TourExecutionStatus.Abandoned)
            .ToList();

        if (!finishedExecutions.Any())
        {
            return new TourStats(tourId, 0, 0);
        }

        // Completion Rate: Completed / (Completed + Abandoned) * 100
        var completedCount = finishedExecutions.Count(te => te.Status == TourExecutionStatus.Completed);
        var completionRate = (double)completedCount / finishedExecutions.Count * 100;

        // Average Completion Percentage
        var avgCompletionPercentage = finishedExecutions.Average(te => te.CompletionPercentage);

        return new TourStats(tourId, completionRate, avgCompletionPercentage);
    }
}
