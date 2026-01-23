using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author
{
    [Authorize(Policy = "authorPolicy")]
    [Route("api/author")]
    [ApiController]
    public class AuthorStatisticsController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly ITourRatingService _tourRatingService;

        public AuthorStatisticsController(ITourService tourService, ITourRatingService tourRatingService)
        {
            _tourService = tourService;
            _tourRatingService = tourRatingService;
        }

        [HttpGet("average-rating")]
        public ActionResult<double> GetAverageRating()
        {
            var authorId = GetAuthorIdFromToken();

            var tours = _tourService.GetByAuthor(authorId) ?? new System.Collections.Generic.List<TourDto>();

            var published = tours
                .Where(t => string.Equals(t.Status, "Published", System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            var allRatings = published
                .SelectMany(t => _tourRatingService.GetByTour((int)t.Id))
                .ToList();

            if (!allRatings.Any())
                return Ok(0.0);

            var avg = allRatings.Average(r => r.Rating);
            var rounded = System.Math.Round(avg, 1);
            return Ok(rounded);
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
}
