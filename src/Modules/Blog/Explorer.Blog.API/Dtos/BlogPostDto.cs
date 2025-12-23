namespace Explorer.Blog.API.Dtos;

public class BlogPostDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorSurname { get; set; }
    public string AuthorUsername { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public int Status { get; set; }
    public int PopularityStatus { get; set; }
    public List<BlogImageDto> Images { get; set; } = new();
    public int UpvoteCount { get; set; }
    public int DownvoteCount { get; set; }
    public VoteDto? UserVote { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}

public class BlogImageDto
{
    public string Url { get; set; }
    public int Order { get; set; }
}
