using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly StakeholdersContext _context;

        public MessageRepository(StakeholdersContext context)
        {
            _context = context;
        }

        public Message Create(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();
            return message;
        }

        public Message Get(long id)
        {
            return _context.Messages.FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<Message> GetConversation(long userId1, long userId2)
        {
            return _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.RecipientId == userId2) ||
                    (m.SenderId == userId2 && m.RecipientId == userId1))
                .OrderBy(m => m.TimestampCreated)
                .ToList();
        }

        public Message Update(Message message)
        {
            _context.Messages.Update(message);
            _context.SaveChanges();
            return message;
        }
    }
}
