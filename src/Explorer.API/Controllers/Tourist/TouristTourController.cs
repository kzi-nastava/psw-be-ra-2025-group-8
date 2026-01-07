using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/tours/public")]
    public class TouristTourController : ControllerBase
    {
        private readonly ITouristTourService _touristTourService;

        public TouristTourController(ITouristTourService touristTourService)
        {
            _touristTourService = touristTourService;
        }

        [HttpGet]
        public IActionResult GetPublishedTours(
            [FromQuery] bool ownedEquipment = false,
            [FromQuery] bool preferenceTags = false,
            [FromQuery] bool preferenceDifficulty = false,
            [FromQuery] List<int>? difficulties = null,
            [FromQuery] int? minPrice = null,
            [FromQuery] int? maxPrice = null
)
        {
            if (minPrice.HasValue && maxPrice.HasValue && maxPrice.Value < minPrice.Value)
                return BadRequest("maxPrice cannot be less than minPrice.");

            var hasDifficultyFilter = difficulties is { Count: > 0 };
            var hasPersonFilters = ownedEquipment || preferenceTags || preferenceDifficulty;
            var hasPriceFilter = minPrice.HasValue || maxPrice.HasValue;


            if (!hasPersonFilters && !hasDifficultyFilter && !hasPriceFilter)
                return Ok(_touristTourService.GetPublishedTours());

            if (!hasPersonFilters && hasDifficultyFilter)
                return Ok(_touristTourService.GetPublishedTours(difficulties!, minPrice, maxPrice));

            if (!hasPersonFilters && hasPriceFilter)
                return Ok(_touristTourService.GetPublishedTours(minPrice, maxPrice));

            if (!TryGetPersonId(out var personId))
                return Unauthorized();


            return Ok(_touristTourService.GetPublishedTours(
                personId,
                ownedEquipment,
                preferenceTags,
                preferenceDifficulty,
                difficulties,
                minPrice,
                maxPrice));

        }


        private bool TryGetPersonId(out long personId)
        {
            personId = 0;
            var claim = User?.Claims?.FirstOrDefault(c => c.Type == "personId");
            if (claim == null) return false;
            return long.TryParse(claim.Value, out personId);
        }


        [HttpGet("{id}")]
        public IActionResult GetPublishedTour(long id)
        {
            var result = _touristTourService.GetPublishedTourDetails(id);
            if (result == null) return NotFound("Tour not found or not published.");
            return Ok(result);
        }

        [HttpGet("{id}/keypoints")]
        public IActionResult GetTourKeyPoints(long id)
        {
            var keyPoints = _touristTourService.GetTourKeyPoints(id);
            if (!keyPoints.Any()) return NotFound("Tour not found or has no keypoints.");
            return Ok(keyPoints);
        }

        [HttpPost("search-by-location")]
        public IActionResult SearchToursByLocation([FromBody] TourSearchByLocationDto searchDto)
        {
            if (searchDto == null)
                return BadRequest("Search parameters are required.");

            if (searchDto.DistanceInKilometers <= 0)
                return BadRequest("Distance must be greater than zero.");

            var tours = _touristTourService.SearchToursByLocation(searchDto);
            return Ok(tours);
        }
    }
}
