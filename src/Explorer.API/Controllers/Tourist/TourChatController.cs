using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tour-chat")]
    [ApiController]
    public class TourChatController : ControllerBase
    {
        private readonly ITourChatRoomService _chatRoomService;

        public TourChatController(ITourChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        private long GetCurrentUserId()
        {
            var idClaim = User.FindFirst("id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null || !long.TryParse(idClaim.Value, out long userId))
            {
                return -1;
            }
            return userId;
        }

        [HttpGet("tour/{tourId:long}")]
        public ActionResult<TourChatRoomDto> GetTourChatRoom(long tourId, [FromQuery] string tourName)
        {
            var chatRoom = _chatRoomService.GetOrCreateTourChatRoom(tourId, tourName);
            return Ok(chatRoom);
        }

        [HttpGet("my-rooms")]
        public ActionResult<List<TourChatRoomDto>> GetMyChatRooms()
        {
            var userId = GetCurrentUserId();
            var rooms = _chatRoomService.GetUserChatRooms(userId);
            return Ok(rooms);
        }

        [HttpGet("{chatRoomId:long}/messages")]
        public ActionResult<List<TourChatMessageDto>> GetMessages(long chatRoomId)
        {
            var userId = GetCurrentUserId();
            var messages = _chatRoomService.GetMessages(chatRoomId, userId);
            return Ok(messages);
        }

        [HttpPost("{chatRoomId:long}/messages")]
        public ActionResult SendMessage(long chatRoomId, [FromBody] string content)
        {
            var userId = GetCurrentUserId();
            _chatRoomService.AddMessage(chatRoomId, userId, content);
            return Ok(new { success = true });
        }
    }
}
