using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IMessageRepository
    {
        Message Create(Message message);
        Message Get(long id);
        IEnumerable<Message> GetConversation(long userId1, long userId2);
        Message Update(Message message);
    }
}
