using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/recommendations")]
[ApiController]
public class TourRecommendationController : ControllerBase
{
    private readonly ITourRecommendationService _recommendationService;

    public TourRecommendationController(ITourRecommendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet]
    public ActionResult<AuthorRecommendationsDto> GetMyRecommendations()
    {
        var authorId = GetAuthorIdFromToken();
        var recommendations = _recommendationService.GetRecommendationsForAuthor(authorId);
        return Ok(recommendations);
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
