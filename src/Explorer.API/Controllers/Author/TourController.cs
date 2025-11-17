using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/tour")]
[ApiController]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [HttpGet]
    public ActionResult<PagedResult<TourDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_tourService.GetPaged(page, pageSize));
    }

    [HttpGet("my-tours")]
    public ActionResult<List<TourDto>> GetMyTours()
    {
        var authorId = GetAuthorId();
        return Ok(_tourService.GetByAuthor(authorId));
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourDto tour)
    {
        var authorId = GetAuthorId();
        tour.AuthorId = authorId;
        return Ok(_tourService.Create(tour));
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update([FromBody] TourDto tour)
    {
        var authorId = GetAuthorId();

        // Ensure caller is the author; use service data instead of trusting input
        var existingTours = _tourService.GetByAuthor(authorId);
        if (!existingTours.Any(t => t.Id == tour.Id))
        {
            throw new UnauthorizedAccessException("You can only update your own tours.");
        }

        // Force AuthorId to current user to avoid spoofing
        tour.AuthorId = authorId;
        var result = _tourService.Update(tour);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var authorId = GetAuthorId();
        _tourService.Delete(id, authorId);
        return Ok();
    }

    private int GetAuthorId()
    {
        // Tests set claim type "personId"; keep backward compatibility with NameIdentifier if present.
        var claim = User.FindFirst("personId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
        return int.Parse(claim?.Value ?? "0");
    }
}