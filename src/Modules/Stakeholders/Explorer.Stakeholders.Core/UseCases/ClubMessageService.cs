using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubMessageService : IClubMessageService
    {
        private readonly IClubMessageRepository _clubMessageRepository;
        private readonly IClubRepository _clubRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IInternalPersonService _personService;
        private readonly IInternalUserService _userService;
        private readonly IMapper _mapper;

        public ClubMessageService(IClubMessageRepository clubMessageRepository, IClubRepository clubRepository, INotificationRepository notificationRepository, IInternalPersonService personService, IInternalUserService userService, IMapper mapper)
        {
            _clubMessageRepository = clubMessageRepository;
            _clubRepository = clubRepository;
            _notificationRepository = notificationRepository;
            _personService = personService;
            _userService = userService;
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

            // ? NEW: Create notifications for all club members (except the sender)
            foreach (var memberId in club.MemberIds.Where(m => m != authorId))
            {
                var notification = new Notification(
                    memberId,
                    NotificationType.ClubActivity,
                    $"New message in {club.Name}",
                    dto.Content.Length > 50 ? dto.Content.Substring(0, 50) + "..." : dto.Content,
                    created.Id,
                    $"ClubMessage:{dto.ClubId}"
                );
                _notificationRepository.Create(notification);
            }

            // Also notify the owner if they're not the sender
            if (club.OwnerId != authorId && !club.MemberIds.Contains(club.OwnerId))
            {
                var ownerNotification = new Notification(
                    club.OwnerId,
                    NotificationType.ClubActivity,
                    $"New message in {club.Name}",
                    dto.Content.Length > 50 ? dto.Content.Substring(0, 50) + "..." : dto.Content,
                    created.Id,
                    $"ClubMessage:{dto.ClubId}"
                );
                _notificationRepository.Create(ownerNotification);
            }

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
            var messageDtos = messages.Select(_mapper.Map<ClubMessageDto>).ToList();
            
            // Enrich with author information
            var authorIds = messageDtos.Select(m => m.AuthorId).Distinct().ToList();
            var personData = _personService.GetByUserIds(authorIds);
            var userData = _userService.GetByIds(authorIds);
            
            foreach (var messageDto in messageDtos)
            {
                if (personData.TryGetValue(messageDto.AuthorId, out var person))
                {
                    messageDto.AuthorName = person.Name;
                    messageDto.AuthorSurname = person.Surname;
                }
                
                if (userData.TryGetValue(messageDto.AuthorId, out var user))
                {
                    messageDto.AuthorUsername = user.Username;
                }
            }
            
            return messageDtos;
        }
    }
}
