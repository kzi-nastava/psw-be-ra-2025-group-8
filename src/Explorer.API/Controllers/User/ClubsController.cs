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
        public ActionResult<ClubDto> Update(long id, [FromBody] ClubDto dto)
        {
            int userId = ExtractUserId();
            var updated = _clubService.Update(id, dto.OwnerId, dto, userId);
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

        // ------ OWNER ACTIONS ------
        // Invite a tourist by username (owner only)
        public class InviteByUsernameDto { public string Username { get; set; } }

        [HttpPost("{id:long}/invite")]
        public ActionResult<ClubInvitationDto> InviteTourist(long id, [FromBody] InviteByUsernameDto dto)
        {
            int ownerId = ExtractUserId();
            var result = _clubService.InviteTouristByUsername(id, ownerId, dto.Username);
            return Ok(result);
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

        // ------ INVITATION ACTIONS ------
        [HttpGet("my-invitations")]
        public ActionResult<IEnumerable<ClubInvitationDto>> GetMyInvitations()
        {
            int touristId = ExtractUserId();
            var invitations = _clubService.GetMyInvitations(touristId);
            return Ok(invitations);
        }

        [HttpGet("{id:long}/invitations")]
        public ActionResult<IEnumerable<ClubInvitationDto>> GetClubInvitations(long id)
        {
            int ownerId = ExtractUserId();
            var invitations = _clubService.GetClubInvitations(id, ownerId);
            return Ok(invitations);
        }

        [HttpPost("invitations/{invitationId:long}/accept")]
        public ActionResult<ClubInvitationDto> AcceptInvitation(long invitationId)
        {
            int touristId = ExtractUserId();
            var result = _clubService.AcceptInvitation(invitationId, touristId);
            return Ok(result);
        }

        [HttpPost("invitations/{invitationId:long}/reject")]
        public ActionResult<ClubInvitationDto> RejectInvitation(long invitationId)
        {
            int touristId = ExtractUserId();
            var result = _clubService.RejectInvitation(invitationId, touristId);
            return Ok(result);
        }

        [HttpDelete("invitations/{invitationId:long}")]
        public ActionResult CancelInvitation(long invitationId)
        {
            int ownerId = ExtractUserId();
            _clubService.CancelInvitation(invitationId, ownerId);
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
