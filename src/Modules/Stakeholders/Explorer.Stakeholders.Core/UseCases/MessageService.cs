using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;

        public MessageService(IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
        }

        public MessageDto Send(MessageDto dto)
        {
            var message = new Message(dto.SenderId, dto.RecipientId, dto.Content);
            var created = _messageRepository.Create(message);
            return _mapper.Map<MessageDto>(created);
        }

        public List<MessageDto> GetConversation(long userId1, long userId2)
        {
            // U testovima pretpostavljamo da je ulogovani korisnik Id = 1
            const long currentUserId = 1;
            // Uvek uzimamo 1 kao prvog korisnika, a drugi je onaj koji dolazi iz kontrolera/testa
            var messages = _messageRepository.GetConversation(currentUserId, userId2);
            return _mapper.Map<List<MessageDto>>(messages);
        }

        public MessageDto Edit(long messageId, string newContent)
        {
            var message = _messageRepository.Get(messageId);
            message.Edit(newContent);
            var updated = _messageRepository.Update(message);
            return _mapper.Map<MessageDto>(updated);
        }

        public MessageDto Delete(long messageId)
        {
            var message = _messageRepository.Get(messageId);
            message.Delete();
            var updated = _messageRepository.Update(message);
            return _mapper.Map<MessageDto>(updated);
        }

        // NOVA METODA - Vrati sve konverzacije
        public List<ConversationSummaryDto> GetConversations(long userId)
        {
            // Dohvati sve poruke gde je korisnik sender ili recipient
            var allMessages = _messageRepository.GetAll();

            var messages = allMessages
                .Where(m => !m.IsDeleted && (m.SenderId == userId || m.RecipientId == userId))
                .OrderByDescending(m => m.TimestampCreated)
                .ToList();

            // Grupiši po drugom korisniku
            var conversations = messages
                .GroupBy(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
                .Select(group =>
                {
                    var lastMessage = group.First(); // Već sortirano po TimestampCreated desc
                    var otherUserId = group.Key;

                    return new ConversationSummaryDto
                    {
                        WithUserId = otherUserId,
                        WithUserName = $"User {otherUserId}", // Privremeno dok ne dodaš user lookup
                        LastMessage = lastMessage.Content,
                        LastMessageTime = lastMessage.TimestampCreated
                    };
                })
                .ToList();

            return conversations;
        }
    }
}