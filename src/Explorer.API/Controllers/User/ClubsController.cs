using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;

namespace Explorer.API.Controllers.User
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/clubs")]
    [ApiController]
    public class ClubsController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubsController(IClubService clubService)
        {
            _clubService = clubService;
        }

        // Create
        [HttpPost]
        public ActionResult<ClubDto> Create([FromBody] CreateClubDto dto)
        {
            int ownerId = ExtractUserId();
            return Ok(_clubService.Create(dto, ownerId));
        }

        // Get by id
        [HttpGet("{id:long}")]
        [AllowAnonymous] // all tourists should be able to view; adjust policy if needed
        public ActionResult<ClubDto> Get(long id)
        {
            return Ok(_clubService.Get(id));
        }

        // List
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<PagedResult<ClubDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        {
            return Ok(_clubService.GetPaged(page, pageSize));
        }

        // Join
        [HttpPost("{id:long}/join")]
        public ActionResult Join(long id)
        {
            int touristId = ExtractUserId();
            _clubService.Join(id, touristId);
            return Ok();
        }

        // Update (owner only)
        [HttpPut("{id:long}")]
        public ActionResult<ClubDto> Update(long id, long current_owner_id, [FromBody] ClubDto dto)
        {
            int userId = ExtractUserId();
            var updated = _clubService.Update(id, current_owner_id, dto, userId);
            return Ok(updated);
        }

        // Delete (owner only)
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            int userId = ExtractUserId();
            _clubService.Delete(userId, id);
            return Ok();
        }

        private int ExtractUserId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "id" ||
                c.Type == "sub" ||
                c.Type == "personId"
            );

            if (claim == null || !int.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User id claim is missing");

            return userId;
        }

    }
}
