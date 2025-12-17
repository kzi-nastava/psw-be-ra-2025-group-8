using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
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
        public Task<ActionResult<ClubDto>> Create([FromBody] CreateClubDto dto)
        {
            long ownerId = ExtractUserId();
            var result = _clubService.Create(dto, ownerId);
            return Task.FromResult<ActionResult<ClubDto>>(Ok(result));
        }

        // Get by id
        [HttpGet("{id:long}")]
        [AllowAnonymous] // all tourists should be able to view; adjust policy if needed
        public Task<ActionResult<ClubDto>> Get(long id)
        {
            var result = _clubService.Get(id);
            return Task.FromResult<ActionResult<ClubDto>>(Ok(result));
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
            long touristId = ExtractUserId();
            _clubService.Join(id, touristId);
            return Ok();
        }

        // Update (owner only)
        [HttpPut("{id:long}")]
        public Task<ActionResult<ClubDto>> Update(long id, long current_owner_id, [FromBody] ClubDto dto)
        {
            long userId = ExtractUserId();
            var updated = _clubService.Update(id, current_owner_id, dto, userId);
            return Task.FromResult<ActionResult<ClubDto>>(Ok(updated));
        }

        // Delete (owner only)
        [HttpDelete("{id:long}")]
        public ActionResult Delete(long id)
        {
            long userId = ExtractUserId();
            _clubService.Delete(userId, id);
            return Ok();
        }

        // ------ OWNER ACTIONS ------
        // Invite a tourist (owner only)
        public class InviteDto { public long TouristId { get; set; } }

        [HttpPost("{id:long}/invite")]
        public ActionResult Invite(long id, [FromBody] InviteDto dto)
        {
            int ownerId = ExtractUserId();
            _clubService.Invite(id, ownerId, dto.TouristId);
            return Ok();
        }

        // Expel a tourist (owner only)
        public class ExpelDto { public long TouristId { get; set; } }

        [HttpPost("{id:long}/expel")]
        public ActionResult Expel(long id, [FromBody] ExpelDto dto)
        {
            int ownerId = ExtractUserId();
            _clubService.Expel(id, ownerId, dto.TouristId);
            return Ok();
        }

        [HttpPost("{id:long}/close")]
        public ActionResult Close(long id)
        {
            int ownerId = ExtractUserId();
            _clubService.Close(id, ownerId);
            return Ok();
        }

        [HttpPost("{id:long}/activate")]
        public ActionResult Activate(long id)
        {
            int ownerId = ExtractUserId();
            _clubService.Activate(id, ownerId);
            return Ok();
        }

        // ------ JOIN REQUEST ACTIONS ------
        [HttpPost("{id:long}/request-join")]
        public ActionResult<ClubJoinRequestDto> RequestToJoin(long id)
        {
            int touristId = ExtractUserId();
            var result = _clubService.RequestToJoin(id, touristId);
            return Ok(result);
        }

        [HttpDelete("join-requests/{requestId:long}")]
        public ActionResult CancelJoinRequest(long requestId)
        {
            int touristId = ExtractUserId();
            _clubService.CancelJoinRequest(requestId, touristId);
            return Ok();
        }

        [HttpPost("join-requests/{requestId:long}/accept")]
        public ActionResult<ClubJoinRequestDto> AcceptJoinRequest(long requestId)
        {
            int ownerId = ExtractUserId();
            var result = _clubService.AcceptJoinRequest(requestId, ownerId);
            return Ok(result);
        }

        [HttpPost("join-requests/{requestId:long}/reject")]
        public ActionResult<ClubJoinRequestDto> RejectJoinRequest(long requestId)
        {
            int ownerId = ExtractUserId();
            var result = _clubService.RejectJoinRequest(requestId, ownerId);
            return Ok(result);
        }

        [HttpGet("{id:long}/join-requests")]
        public ActionResult<IEnumerable<ClubJoinRequestDto>> GetClubJoinRequests(long id)
        {
            int ownerId = ExtractUserId();
            var requests = _clubService.GetClubJoinRequests(id, ownerId);
            return Ok(requests);
        }

        [HttpGet("my-join-requests")]
        public ActionResult<IEnumerable<ClubJoinRequestDto>> GetMyJoinRequests()
        {
            int touristId = ExtractUserId();
            var requests = _clubService.GetMyJoinRequests(touristId);
            return Ok(requests);
        }

        private long ExtractUserId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "id" ||
                c.Type == "sub" ||
                c.Type == "personId"
            );

            if (claim == null || !long.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User id claim is missing");

            return userId;
        }

    }
}
