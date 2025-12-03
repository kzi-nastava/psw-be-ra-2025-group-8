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
[Route("api/blogs")] // Globalna ruta za blogove
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
            // Blog sa tim ID-jem ne postoji
            return NotFound($"Blog post with ID {blogId} not found.");
        }
    }

    /// <summary>
    /// Create a new comment on a specific blog post (requires authentication)
    /// </summary>
    [HttpPost("{blogId:long}/comments")]
    [Authorize(Policy = "personPolicy")] // Ograničavamo samo na ulogovane osobe
    public ActionResult<CommentDto> Create([FromRoute] long blogId, [FromBody] CommentCreationDto commentData)
    {
        var personId = User.PersonId();

        try
        {
            var created = _blogCommentService.Create(blogId, personId, commentData);

            // 201 Created je standardan odgovor za uspešno kreiranje resursa.
            // Povezujemo ga sa GET endpointom (iako vraćamo ceo objekat)
            return CreatedAtAction(nameof(GetCommentsForBlog), new { blogId = blogId }, created);
        }
        catch (KeyNotFoundException)
        {
            // Blog nije pronađen
            return NotFound($"Blog post with ID {blogId} not found.");
        }
        catch (UnauthorizedAccessException e)
        {
            // Hvatanje domenske greške iz servisa (npr. "Comments can only be added to a Published blog.")
            return StatusCode(403, e.Message); // 403 Forbidden
        }
        catch (ArgumentException e)
        {
            // Hvatanje domenske greške (npr. prazan tekst komentara - validacija iz Comment entiteta)
            return BadRequest(e.Message); // 400 Bad Request
        }
    }
}