namespace Explorer.Blog.API.Dtos;

public class VoteDto
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public DateTime CreatedAt { get; set; }
    public int Type { get; set; } // VoteType enum as int
}

