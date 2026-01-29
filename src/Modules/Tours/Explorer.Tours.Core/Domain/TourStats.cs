using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourStats : Entity
{
    public long TourId { get; private set; }

    // Completion Statistics
    public double CompletionRate { get; private set; } // Completed / (Completed + Abandoned) in percentage
    public double AverageCompletionPercentage { get; private set; } // Average of CompletionPercentage from TourExecutions
    public DifficultyLevel mostCommonDiffcultyLevel { get; private set; } //Beginner, Intermediate, Professional
    public TimeSpan AverageDuration { get; private set; }

    // Metadata
    public DateTime LastUpdated { get; private set; }

    // Navigation property
    public Tour Tour { get; private set; }

    public TourStats() { }

    public TourStats(long tourId, double completionRate, double averageCompletionPercentage, DifficultyLevel mostCommonDifficultyLevel, TimeSpan averageDuration = default)
    {
        TourId = tourId;
        CompletionRate = completionRate;
        AverageCompletionPercentage = averageCompletionPercentage;
        mostCommonDiffcultyLevel = mostCommonDifficultyLevel;
        AverageDuration = averageDuration;
        LastUpdated = DateTime.UtcNow;

        Validate();
    }

    public void Update(double completionRate, double averageCompletionPercentage, DifficultyLevel mostCommonDifficultyLevel, TimeSpan averageDuration = default)
    {
        CompletionRate = completionRate;
        AverageCompletionPercentage = averageCompletionPercentage;
        mostCommonDiffcultyLevel = mostCommonDifficultyLevel;
        AverageDuration = averageDuration;
        LastUpdated = DateTime.UtcNow;

        Validate();
    }

    private void Validate()
    {
        if (CompletionRate < 0 || CompletionRate > 100)
            throw new ArgumentException("Completion rate must be between 0 and 100.");

        if (AverageCompletionPercentage < 0 || AverageCompletionPercentage > 100)
            throw new ArgumentException("Average completion percentage must be between 0 and 100.");
    }

    public bool IsStale()
    {
        return LastUpdated < DateTime.UtcNow.AddDays(-1);
    }
}
