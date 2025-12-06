namespace Explorer.Blog.API.Dtos;

public class CreateBlogPostDto
{
    public long AuthorId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<BlogImageDto> Images { get; set; } = new();
}
