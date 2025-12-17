using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class ReportProblem : Entity
{
    public int TourId { get; set; }
    public int TouristId { get; set; }
    public ReportCategory Category { get; set; }
    public ReportPriority Priority { get; set; }
    public string Description { get; set; }
    public DateTime ReportTime { get; set; }

    // Novi podaci za odgovore i rezoluciju
    public int? AuthorId { get; set; }
    public string? AuthorResponse { get; set; }
    public DateTime? AuthorResponseTime { get; set; }

    public bool? IsResolved { get; set; }
    public string? TouristResolutionComment { get; set; }
    public DateTime? TouristResolutionTime { get; set; }

    // Poruke u okviru prijave problema
    public List<IssueMessage> Messages { get; set; } = new List<IssueMessage>();

    public ReportProblem(int tourId, int touristId, ReportCategory category, ReportPriority priority, string description)
    {
        if (tourId == 0) throw new ArgumentException("Invalid TourId.");
        if (touristId <= 0) throw new ArgumentException("Invalid TouristId.");
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");

        TourId = tourId;
        TouristId = touristId;
        Category = category;
        Priority = priority;
        Description = description;
        ReportTime = DateTime.UtcNow;
    }

    // prazan konstruktor za EF
    public ReportProblem() { }

    // Metoda kojom autor odgovara na prijavu
    public void RespondByAuthor(int authorId, string response)
    {
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId.");
        if (string.IsNullOrWhiteSpace(response)) throw new ArgumentException("Invalid response.");

        AuthorId = authorId;
        AuthorResponse = response;
        AuthorResponseTime = DateTime.UtcNow;
    }

    // Metoda kojom turista ozna?ava rešenost/nerešenost
    public void MarkResolved(bool resolved, string? comment)
    {
        IsResolved = resolved;
        TouristResolutionComment = comment;
        TouristResolutionTime = DateTime.UtcNow;
    }

    // Metoda za dodavanje poruke
    public void AddMessage(int authorId, string content)
    {
        var message = new IssueMessage(Id, authorId, content);
        Messages.Add(message);
    }

    // Provera da li je problem star više od 5 dana i nije rešen
    public bool IsOverdue()
    {
        var daysSinceReport = (DateTime.UtcNow - ReportTime).TotalDays;
        return daysSinceReport > 5 && IsResolved != true;
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
