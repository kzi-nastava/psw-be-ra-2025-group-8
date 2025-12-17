using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class FollowerMessageRepository : IFollowerMessageRepository
{
    private readonly StakeholdersContext _context;

    public FollowerMessageRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public FollowerMessage Create(FollowerMessage message)
    {
        _context.FollowerMessages.Add(message);
        _context.SaveChanges();
        return message;
    }

    public FollowerMessage Get(long id)
    {
        return _context.FollowerMessages.Find(id);
    }

    public List<FollowerMessage> GetBySenderId(long senderId)
    {
        return _context.FollowerMessages
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.SentAt)
                .ToList();
    }
}
