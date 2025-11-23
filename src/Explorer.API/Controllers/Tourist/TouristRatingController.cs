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
        public ActionResult<RatingDto> Create([FromBody] RatingNoIdDto rating)
        {
            var result = _ratingService.Create(rating, GetLoggedInUserId());
            return Ok(result);
        }

        [HttpGet("user")]
        public ActionResult<RatingDto> GetByCurrentUser()
        {
            var result = _ratingService.GetByUserId(GetLoggedInUserId());
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPut("user")]
        public ActionResult<RatingDto> UpdateByCurrentUser([FromBody] RatingNoIdDto rating)
        {
            var result = _ratingService.UpdateByUserId(rating, GetLoggedInUserId());
            return Ok(result);
        }

        [HttpDelete("user")]
        public ActionResult DeleteByCurrentUser()
        {
            _ratingService.DeleteByUserId(GetLoggedInUserId());
            return Ok();
        }
    }
}