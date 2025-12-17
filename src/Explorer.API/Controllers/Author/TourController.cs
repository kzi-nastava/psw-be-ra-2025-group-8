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
        var authorId = GetAuthorIdFromToken();
        return Ok(_tourService.GetByAuthor(authorId));
    }

    [HttpGet("{id:long}")]
    public ActionResult<TourDto> GetById(long id)
    {
        var authorId = GetAuthorIdFromToken();

        var tours = _tourService.GetByAuthor(authorId);
        var tour = tours.FirstOrDefault(t => t.Id == id);

        if (tour == null)
        {
            return NotFound("Tour not found or you don't have permission to access it");
        }

        return Ok(tour);
    }

    [HttpPost]
    public ActionResult<TourDto> Create([FromBody] TourDto tour)
    {
        var authorId = GetAuthorIdFromToken();
        tour.AuthorId = authorId;
        var result = _tourService.Create(tour);
        return Ok(result);
    }

    [HttpPut("{id:long}")]
    public ActionResult<TourDto> Update([FromBody] TourDto tour)
    {
        var authorId = GetAuthorIdFromToken();

        var existingTours = _tourService.GetByAuthor(authorId);
        if (!existingTours.Any(t => t.Id == tour.Id))
        {
            throw new UnauthorizedAccessException("You cannot modify another author's tour");
        }

        var result = _tourService.Update(tour);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        var authorId = GetAuthorIdFromToken();
        _tourService.Delete(id, authorId);
        return Ok();
    }

    // ============================================================
    // Authoring endpoints for key points and tour lifecycle
    // ============================================================

    // POST api/author/tour/{tourId}/key-points
    // Adding a key point to a tour
    [HttpPost("{tourId:long}/key-points")]
    public ActionResult<TourDto> AddKeyPoint(long tourId, [FromBody] KeyPointDto keyPoint)
    {
        var authorId = GetAuthorIdFromToken();
        var result = _tourService.AddKeyPoint(tourId, keyPoint, authorId);
        return Ok(result);
    }

    // PUT api/author/tour/{id}/publish
    // Publishing a tour
    [HttpPut("{id:long}/publish")]
    public ActionResult<TourDto> Publish(long id)
    {
        var authorId = GetAuthorIdFromToken();
        var result = _tourService.Publish(id, authorId);
        return Ok(result);
    }

    // PUT api/author/tour/{id}/archive
    // Archiving a tour
    [HttpPut("{id:long}/archive")]
    public ActionResult<TourDto> Archive(long id)
    {
        var authorId = GetAuthorIdFromToken();
        var result = _tourService.Archive(id, authorId);
        return Ok(result);
    }

    // PUT api/author/tour/{id}/reactivate
    // Reactivating an archived tour
    [HttpPut("{id:long}/reactivate")]
    public ActionResult<TourDto> Reactivate(long id) 
    {
        var authorId = GetAuthorIdFromToken();
        var result = _tourService.Reactivate(id, authorId);
        return Ok(result);
    }


    private int GetAuthorIdFromToken()
    {
        var idClaim = User.FindFirst("id")
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)
                   ?? User.FindFirst("personId")
                   ?? User.FindFirst("sub");

        if (idClaim != null && int.TryParse(idClaim.Value, out int authorId))
        {
            return authorId;
        }

        throw new UnauthorizedAccessException("Unable to determine user ID from token");
    }
}
