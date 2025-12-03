namespace Explorer.Blog.API.Dtos;

public class BlogPostDto
{
    public long Id { get; set; }
    public long AuthorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public int Status { get; set; }
    public List<BlogImageDto> Images { get; set; } = new();
}

public class BlogImageDto
{
    public string Url { get; set; }
    public int Order { get; set; }
}
