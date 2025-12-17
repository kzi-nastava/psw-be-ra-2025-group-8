using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubMessageService
    {
        ClubMessageDto PostMessage(CreateClubMessageDto dto, long authorId);
        ClubMessageDto UpdateMessage(long messageId, UpdateClubMessageDto dto, long userId);
        void DeleteMessage(long messageId, long userId);
        IEnumerable<ClubMessageDto> GetClubMessages(long clubId);
    }
}
