using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class IssueMessage : Entity
{
    public long ReportProblemId { get; private set; }
    public int AuthorId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IssueMessage(long reportProblemId, int authorId, string content)
    {
        if (reportProblemId == 0) throw new ArgumentException("Invalid ReportProblemId.");
        if (authorId == 0) throw new ArgumentException("Invalid AuthorId.");
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Content cannot be empty.");

        ReportProblemId = reportProblemId;
        AuthorId = authorId;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    // Prazan konstruktor za EF Core
    private IssueMessage() { }
}
