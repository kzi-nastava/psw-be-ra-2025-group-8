using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Authorize(Policy = "authorPolicy")]
    [Route("api/rating")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public ActionResult<RatingDto> Create([FromBody] RatingDto rating)
        {
            var result = _ratingService.Create(rating);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<RatingDto> Get(int id)
        {
            var result = _ratingService.Get(id);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public ActionResult<RatingDto> Update([FromBody] RatingDto rating)
        {
            var result = _ratingService.Update(rating);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            _ratingService.Delete(id);
            return Ok();
        }
    }
}
