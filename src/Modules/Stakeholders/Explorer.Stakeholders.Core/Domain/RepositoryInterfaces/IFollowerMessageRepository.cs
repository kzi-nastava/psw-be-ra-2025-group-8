using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IFollowerMessageRepository
{
    FollowerMessage Create(FollowerMessage message);
    FollowerMessage Get(long id);
    List<FollowerMessage> GetBySenderId(long senderId);
}
