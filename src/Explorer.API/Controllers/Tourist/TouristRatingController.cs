using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristAuthorPolicy")]
    [Route("api/rating")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        private long GetLoggedInUserId()
        {

            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (idClaim != null && long.TryParse(idClaim.Value, out long userId))
            {
                return userId;
            }
            throw new InvalidOperationException("User ID claim (id) is missing or invalid.");
        }

        [HttpPost]
        public ActionResult<RatingDto> Create([FromBody] RatingDto rating)
        {
            var result = _ratingService.Create(rating, GetLoggedInUserId());
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
            var result = _ratingService.Update(rating, GetLoggedInUserId());
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            _ratingService.Delete(id, GetLoggedInUserId());
            return Ok();
        }
    }
}