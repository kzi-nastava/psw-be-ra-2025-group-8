using System.Collections.Generic;
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

        public IList<MessageDto> GetConversation(long userId1, long userId2)
        {
            var messages = _messageRepository.GetConversation(userId1, userId2);
            return _mapper.Map<IList<MessageDto>>(messages);
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
    }
}
