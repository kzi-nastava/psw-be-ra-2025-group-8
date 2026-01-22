using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Linq;
using static Explorer.Tours.Core.Domain.TourExecution;

namespace Explorer.Tours.Core.UseCases.Author;

public class TourStatsService : ITourStatsService
{
    private readonly ITourStatsRepository _tourStatsRepository;
    private readonly ITourExecutionRepository _tourExecutionRepository;
    private readonly ITouristPreferencesRepository _touristPreferencesRepository;
    private readonly IMapper _mapper;

    public TourStatsService(
        ITourStatsRepository tourStatsRepository,
        ITourExecutionRepository tourExecutionRepository,
        ITouristPreferencesRepository touristPreferencesRepository,
        IMapper mapper)
    {
        _tourStatsRepository = tourStatsRepository;
        _tourExecutionRepository = tourExecutionRepository;
        _touristPreferencesRepository = touristPreferencesRepository;
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
                existingStats.Update(calculatedStats.CompletionRate, calculatedStats.AverageCompletionPercentage, calculatedStats.mostCommonDiffcultyLevel);
                existingStats = _tourStatsRepository.Update(existingStats);
            }
        }

        return _mapper.Map<TourStatsDto>(existingStats);
    }

    private TourStats CalculateTourStats(int tourId)
    {
        var allExecutions = _tourExecutionRepository.GetByTour(tourId);
        var allPreferences = _touristPreferencesRepository.GetAll();

        // Get unique tourist IDs from executions
        var personIds = allExecutions.Select(te => te.IdTourist).Distinct().ToList();

        // Count preferences only for tourists who actually used this tour
        var BeginnersCount = allPreferences.Where(p => personIds.Contains((int)p.PersonId) && p.Difficulty == DifficultyLevel.Beginner).Count();
        var IntermediateCount = allPreferences.Where(p => personIds.Contains((int)p.PersonId) && p.Difficulty == DifficultyLevel.Intermediate).Count();
        var ProfessionalCount = allPreferences.Where(p => personIds.Contains((int)p.PersonId) && p.Difficulty == DifficultyLevel.Professional).Count();

        // Determine most common difficulty level (prioritize in order: Beginner, Intermediate, Professional for ties)
        var mostCommonDifficulty = DifficultyLevel.Beginner; // default when no preferences exist

        if (BeginnersCount > 0 || IntermediateCount > 0 || ProfessionalCount > 0)
        {
            if (BeginnersCount >= IntermediateCount && BeginnersCount >= ProfessionalCount)
                mostCommonDifficulty = DifficultyLevel.Beginner;
            else if (IntermediateCount >= ProfessionalCount)
                mostCommonDifficulty = DifficultyLevel.Intermediate;
            else
                mostCommonDifficulty = DifficultyLevel.Professional;
        }


        // Filter only Completed and Abandoned (exclude InProgress)
        var finishedExecutions = allExecutions
            .Where(te => te.Status == TourExecutionStatus.Completed ||
                         te.Status == TourExecutionStatus.Abandoned)
            .ToList();

        if (!finishedExecutions.Any())
        {
            return new TourStats(tourId, 0, 0, mostCommonDifficulty);
        }

        // Completion Rate: Completed / (Completed + Abandoned) * 100
        var completedCount = finishedExecutions.Count(te => te.Status == TourExecutionStatus.Completed);
        var completionRate = (double)completedCount / finishedExecutions.Count * 100;

        // Average Completion Percentage
        var avgCompletionPercentage = finishedExecutions.Average(te => te.CompletionPercentage);

        return new TourStats(tourId, completionRate, avgCompletionPercentage, mostCommonDifficulty);
    }
}
