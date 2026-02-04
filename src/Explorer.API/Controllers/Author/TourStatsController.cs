using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/tour")]
[ApiController]
public class TourStatsController : ControllerBase
{
    private readonly ITourStatsService _tourStatsService;
    private readonly ITourService _tourService;

    public TourStatsController(ITourStatsService tourStatsService, ITourService tourService)
    {
        _tourStatsService = tourStatsService;
        _tourService = tourService;
    }

    [HttpGet("{tourId:int}/stats")]
    public ActionResult<TourStatsDto> GetTourStats(int tourId)
    {
        var authorId = GetAuthorIdFromToken();

        // Verify that the author owns this tour
        var authorTours = _tourService.GetByAuthor(authorId);
        var tour = authorTours.FirstOrDefault(t => t.Id == tourId);

        if (tour == null)
        {
            return NotFound("Tour not found or you don't have permission to access its statistics");
        }

        var stats = _tourStatsService.GetTourStats(tourId);
        return Ok(stats);
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
