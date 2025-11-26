using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ReportProblem : Entity
{
    public int TourId { get; init; }
    public int TouristId { get; init; }
    public ReportCategory Category { get; init; }
    public ReportPriority Priority { get; init; }
    public string Description { get; init; }
    public DateTime ReportTime { get; init; }

    public ReportProblem(int tourId, int touristId, ReportCategory category, ReportPriority priority, string description)
    {
        if (tourId <= 0) throw new ArgumentException("Invalid TourId.");
        if (touristId <= 0) throw new ArgumentException("Invalid TouristId.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");

        TourId = tourId;
        TouristId = touristId;
        Category = category;
        Priority = priority;
        Description = description;
        ReportTime = DateTime.UtcNow;
    }
}

public enum ReportCategory
{
    Technical = 0,
    Safety = 1,
    Guide = 2,
    Location = 3,
    Other = 4
}

public enum ReportPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}
