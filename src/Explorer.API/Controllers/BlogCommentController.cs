using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Explorer.Stakeholders.Infrastructure.Authentication;
using System;
using System.Linq;

namespace Explorer.API.Controllers;

[ApiController]
[Route("api/blogs")] // global route for blogs
public class BlogCommentController : ControllerBase
{
    private readonly IBlogCommentService _blogCommentService;

    public BlogCommentController(IBlogCommentService blogCommentService)
    {
        _blogCommentService = blogCommentService;
    }

    /// <summary>
    /// Get all comments for a specific blog post (public endpoint)
    /// </summary>
    [HttpGet("{blogId:long}/comments")]
    [AllowAnonymous]
    public ActionResult<List<CommentDto>> GetCommentsForBlog([FromRoute] long blogId)
    {
        try
        {
            var result = _blogCommentService.GetCommentsForBlog(blogId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Blog post with ID {blogId} not found.");
        }
    }

    /// <summary>
    /// Create a new comment on a specific blog post (requires authentication)
    /// </summary>
    [HttpPost("{blogId:long}/comments")]
    [Authorize(Policy = "personPolicy")] // restriction for users(toursit or author)
    public ActionResult<CommentDto> Create([FromRoute] long blogId, [FromBody] CommentCreationDto commentData)
    {
        var personId = User.PersonId();

        try
        {
            var created = _blogCommentService.Create(blogId, personId, commentData);

            // 201 Created
            return CreatedAtAction(nameof(GetCommentsForBlog), new { blogId = blogId }, created);
        }
        catch (KeyNotFoundException)
        {
            // Blog not found
            return NotFound($"Blog post with ID {blogId} not found.");
        }
        catch (UnauthorizedAccessException e)
        {
            // Comments can only be added to a Published blog
            return StatusCode(403, e.Message); // 403 Forbidden
        }
        catch (ArgumentException e)
        {
            // empty text field
            return BadRequest(e.Message); // 400 Bad Request
        }
    }
}