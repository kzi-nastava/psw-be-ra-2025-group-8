using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/tour-rating")]
    [ApiController]
    public class TourRatingController : ControllerBase
    {
        private readonly ITourRatingService _tourRatingService;

        public TourRatingController(ITourRatingService tourRatingService)
        {
            _tourRatingService = tourRatingService;
        }

        [HttpGet]
        public ActionResult<PagedResult<TourRatingDto>> GetAll([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
        {
            var result = _tourRatingService.GetPaged(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<TourRatingDto> Get(int id)
        {
            var result = _tourRatingService.Get(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("tourist/{touristId:int}")]
        public ActionResult<List<TourRatingDto>> GetByTourist(int touristId)
        {
            var result = _tourRatingService.GetByTourist(touristId);
            return Ok(result);
        }

        [HttpGet("tour/{tourId:int}")]
        public ActionResult<List<TourRatingDto>> GetByTour(int tourId)
        {
            var result = _tourRatingService.GetByTour(tourId);
            return Ok(result);
        }

        [HttpGet("tourist/{touristId:int}/tour/{tourId:int}")]
        public ActionResult<TourRatingDto> GetByTouristAndTour(int touristId, int tourId)
        {
            var result = _tourRatingService.GetByTouristAndTour(touristId, tourId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<TourRatingDto> Create([FromBody] TourRatingDto tourRating)
        {
            var result = _tourRatingService.Create(tourRating);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id:long}")]
        public ActionResult<TourRatingDto> Update(long id, [FromBody] TourRatingDto tourRating)
        {
            tourRating.Id = id;
            var result = _tourRatingService.Update(tourRating);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            _tourRatingService.Delete(id);
            return Ok();
        }
    }
}
