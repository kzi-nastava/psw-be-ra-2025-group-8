using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Public;

[AllowAnonymous]
[Route("api/public/blog-posts")]
[ApiController]
public class BlogPostController : ControllerBase
{
    private readonly IBlogPostService _blogPostService;

    public BlogPostController(IBlogPostService blogPostService)
    {
        _blogPostService = blogPostService;
    }

    [HttpGet]
    public ActionResult<List<BlogPostDto>> GetPublicBlogs()
    {
        var result = _blogPostService.GetVisibleBlogs(null);
        return Ok(result);
    }
}
