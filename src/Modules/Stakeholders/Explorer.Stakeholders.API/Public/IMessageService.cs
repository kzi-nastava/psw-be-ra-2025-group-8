using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IMessageService
    {
        MessageDto Send(MessageDto dto);
        List<MessageDto> GetConversation(long userId, long otherUserId);
        MessageDto Edit(long messageId, string newContent);
        MessageDto Delete(long messageId);

        // NOVA METODA
        List<ConversationSummaryDto> GetConversations(long userId);
    }
}