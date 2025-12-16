using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author
{
    [Route("api/author/tours/{tourId}/transport-times")]
    [ApiController]
    [Authorize(Roles = "author")]
    public class TourTransportTimesController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourTransportTimesController(ITourService tourService)
        {
            _tourService = tourService;
        }

        // ============================================================
        //  PUT api/author/tours/{tourId}/transport-times
        //  Set or update transport times for a tour
        // ============================================================
        [HttpPut]
        public ActionResult<TourDto> UpdateTransportTimes(long tourId, [FromBody] UpdateTourTransportTimesDto dto)
        {
            int authorId = GetAuthorIdFromToken();

            var result = _tourService.UpdateTransportTimes(tourId, dto.TransportTimes, authorId);
            return Ok(result);
        }

        // ============================================================
        //  GET api/author/tours/{tourId}/transport-times
        //  Returns transport times for a tour
        // ============================================================
        [HttpGet]
        public ActionResult<List<TourTransportTimeDto>> GetTransportTimes(long tourId)
        {
            int authorId = GetAuthorIdFromToken();

            var tours = _tourService.GetByAuthor(authorId);
            var tour = tours.FirstOrDefault(t => t.Id == tourId);

            if (tour == null) return NotFound();

            return Ok(tour.TransportTimes ?? new List<TourTransportTimeDto>());
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
