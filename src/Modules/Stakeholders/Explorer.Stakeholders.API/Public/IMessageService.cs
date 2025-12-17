using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMessageService
    {
        MessageDto Send(MessageDto dto);
        List<MessageDto> GetConversation(long userId1, long userId2);
        MessageDto Edit(long messageId, string newContent);
        MessageDto Delete(long messageId);
        List<ConversationSummaryDto> GetConversations(long userId);
        void DeleteConversation(long userId, long otherUserId);  // NOVA LINIJA
    }
}