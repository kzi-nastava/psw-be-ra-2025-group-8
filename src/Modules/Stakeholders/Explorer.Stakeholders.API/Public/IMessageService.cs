using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMessageService
    {
        MessageDto Send(MessageDto dto);
        IList<MessageDto> GetConversation(long userId1, long userId2);
        MessageDto Edit(long messageId, string newContent);
        MessageDto Delete(long messageId);
    }
}
