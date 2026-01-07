using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist
{
    public interface ITourChatRoomService
    {
        TourChatRoomDto GetOrCreateTourChatRoom(long tourId, string tourName);
        void AddUserToTourChat(long tourId, long userId);
        void RemoveUserFromTourChat(long tourId, long userId);
        List<TourChatRoomDto> GetUserChatRooms(long userId);
        void AddMessage(long chatRoomId, long senderId, string content);
        List<TourChatMessageDto> GetMessages(long chatRoomId, long userId);
    }
}
