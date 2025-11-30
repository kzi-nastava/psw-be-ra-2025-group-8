using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;  // ✅ DODAJ
        private readonly ICrudRepository<Person> _personRepository;  // ✅ DODAJ

        public MessageService(
            IMessageRepository messageRepository,
            IMapper mapper,
            IUserRepository userRepository,  // ✅ DODAJ
            ICrudRepository<Person> personRepository)  // ✅ DODAJ
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _userRepository = userRepository;  // ✅ DODAJ
            _personRepository = personRepository;  // ✅ DODAJ
        }

        public MessageDto Send(MessageDto dto)
        {
            var message = new Message(dto.SenderId, dto.RecipientId, dto.Content);
            var created = _messageRepository.Create(message);
            return _mapper.Map<MessageDto>(created);
        }

        public List<MessageDto> GetConversation(long userId1, long userId2)
        {
            var messages = _messageRepository.GetConversation(userId1, userId2);
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

        // ✅ ISPRAVLJENA METODA - Vrati sve konverzacije sa pravim imenima
        public List<ConversationSummaryDto> GetConversations(long userId)
        {
            var allMessages = _messageRepository.GetAll();
            var messages = allMessages
                .Where(m => !m.IsDeleted && (m.SenderId == userId || m.RecipientId == userId))
                .OrderByDescending(m => m.TimestampCreated)
                .ToList();

            var conversations = messages
                .GroupBy(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
                .Select(group =>
                {
                    var lastMessage = group.First();
                    var otherUserId = group.Key;

                    // ✅ Pokušaj da učitaš pravo ime korisnika
                    string userName = $"User {otherUserId}";  // fallback
                    try
                    {
                        var user = _userRepository.GetById(otherUserId);
                        if (user != null)
                        {
                            userName = user.Username;

                            // Pokušaj da učitaš ime i prezime iz Person tabele
                            try
                            {
                                var personId = _userRepository.GetPersonId(otherUserId);
                                var person = _personRepository.Get(personId);
                                if (person != null && !string.IsNullOrWhiteSpace(person.Name))
                                {
                                    userName = $"{person.Name} {person.Surname}".Trim();
                                    if (string.IsNullOrWhiteSpace(userName))
                                    {
                                        userName = user.Username;
                                    }
                                }
                            }
                            catch
                            {
                                // Ako nema Person, koristi Username
                            }
                        }
                    }
                    catch
                    {
                        // Ako ne može da učita korisnika, koristi fallback
                    }

                    return new ConversationSummaryDto
                    {
                        WithUserId = otherUserId,
                        WithUserName = userName,
                        LastMessage = lastMessage.Content,
                        LastMessageTime = lastMessage.TimestampCreated
                    };
                })
                .ToList();

            return conversations;
        }

        public void DeleteConversation(long userId, long otherUserId)
        {
            var allMessages = _messageRepository.GetAll();
            var messages = allMessages
                .Where(m => (m.SenderId == userId && m.RecipientId == otherUserId) ||
                            (m.SenderId == otherUserId && m.RecipientId == userId))
                .ToList();

            foreach (var message in messages)
            {
                message.Delete();
                _messageRepository.Update(message);
            }
        }
    }
}