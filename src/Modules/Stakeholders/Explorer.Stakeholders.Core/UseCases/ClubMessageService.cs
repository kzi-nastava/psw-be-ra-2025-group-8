using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubMessageService : IClubMessageService
    {
        private readonly IClubMessageRepository _clubMessageRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubMessageService(IClubMessageRepository clubMessageRepository, IClubRepository clubRepository, IMapper mapper)
        {
            _clubMessageRepository = clubMessageRepository;
            _clubRepository = clubRepository;
            _mapper = mapper;
        }

        public ClubMessageDto PostMessage(CreateClubMessageDto dto, long authorId)
        {
            var club = _clubRepository.Get(dto.ClubId);
            
            if (!club.MemberIds.Contains(authorId) && club.OwnerId != authorId)
            {
                throw new UnauthorizedAccessException("Only club members can post messages");
            }

            var message = new ClubMessage(dto.ClubId, authorId, dto.Content);
            var created = _clubMessageRepository.Create(message);
            return _mapper.Map<ClubMessageDto>(created);
        }

        public ClubMessageDto UpdateMessage(long messageId, UpdateClubMessageDto dto, long userId)
        {
            var message = _clubMessageRepository.Get(messageId);
            
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found");
            }

            if (message.AuthorId != userId)
            {
                throw new UnauthorizedAccessException("Only the author can update their message");
            }

            message.Update(dto.Content);
            var updated = _clubMessageRepository.Update(message);
            return _mapper.Map<ClubMessageDto>(updated);
        }

        public void DeleteMessage(long messageId, long userId)
        {
            var message = _clubMessageRepository.Get(messageId);
            
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found");
            }

            var club = _clubRepository.Get(message.ClubId);
            
            if (message.AuthorId != userId && club.OwnerId != userId)
            {
                throw new UnauthorizedAccessException("Only the author or club owner can delete this message");
            }

            _clubMessageRepository.Delete(messageId);
        }

        public IEnumerable<ClubMessageDto> GetClubMessages(long clubId)
        {
            var messages = _clubMessageRepository.GetByClubId(clubId);
            return messages.Select(_mapper.Map<ClubMessageDto>).ToList();
        }
    }
}
