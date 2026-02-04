using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/summary-stats")]
[ApiController]
public class AuthorSummaryStatsController : ControllerBase
{
    private readonly IAuthorSummaryStatsService _summaryStatsService;

    public AuthorSummaryStatsController(IAuthorSummaryStatsService summaryStatsService)
    {
        _summaryStatsService = summaryStatsService;
    }

    [HttpGet("quick")]
    public ActionResult<AuthorQuickStatsDto> GetQuickStats()
    {
        var authorId = GetAuthorIdFromToken();
        var stats = _summaryStatsService.GetQuickStats(authorId);
        return Ok(stats);
    }

    [HttpGet("trends")]
    public ActionResult<AuthorTourTrendsDto> GetRatingTrends()
    {
        var authorId = GetAuthorIdFromToken();
        var trends = _summaryStatsService.GetRatingTrends(authorId);
        return Ok(trends);
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
