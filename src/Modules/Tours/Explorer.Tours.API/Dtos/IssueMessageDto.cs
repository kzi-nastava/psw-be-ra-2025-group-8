namespace Explorer.Tours.API.Dtos;

public class IssueMessageDto
{
    public int Id { get; set; }
    public long ReportProblemId { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}
