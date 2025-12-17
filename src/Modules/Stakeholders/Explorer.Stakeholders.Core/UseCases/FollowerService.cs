using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class FollowerService : IFollowerService
{
    private readonly IFollowerRepository _followerRepository;
    private readonly IFollowerMessageRepository _messageRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPersonService _personService;
    private readonly IMapper _mapper;

    public FollowerService(
        IFollowerRepository followerRepository,
        IFollowerMessageRepository messageRepository,
        INotificationRepository notificationRepository,
        IPersonService personService,
        IMapper mapper)
    {
        _followerRepository = followerRepository;
        _messageRepository = messageRepository;
        _notificationRepository = notificationRepository;
        _personService = personService;
        _mapper = mapper;
    }

    public FollowerDto Follow(long userId, long followingUserId)
    {
        // Check if already following
        var existing = _followerRepository.GetByUserIds(userId, followingUserId);
        if (existing != null)
            throw new InvalidOperationException("Already following this user.");

        var follower = new Follower(userId, followingUserId);
        var created = _followerRepository.Create(follower);

        // Map to DTO with person info
        var person = _personService.GetByUserId(userId);
        return new FollowerDto
            {
                Id = created.Id,
                UserId = created.UserId,
                Name = $"{person.Name} {person.Surname}",
                FollowedAt = created.FollowedAt
            };
    }

    public void Unfollow(long userId, long followingUserId)
    {
        var follower = _followerRepository.GetByUserIds(userId, followingUserId);
        if (follower == null)
            throw new KeyNotFoundException("Not following this user.");

        _followerRepository.Delete(follower.Id);
    }

    public List<FollowerDto> GetFollowers(long userId)
    {
        var followers = _followerRepository.GetFollowersByUserId(userId);
        return followers.Select(f =>
            {
                var person = _personService.GetByUserId(f.UserId);
                return new FollowerDto
                    {
                        Id = f.Id,
                        UserId = f.UserId,
                        Name = $"{person.Name} {person.Surname}",
                        FollowedAt = f.FollowedAt
                    };
        }).ToList();
    }

    public List<FollowerDto> GetFollowing(long userId)
    {
        var following = _followerRepository.GetFollowingByUserId(userId);
        return following.Select(f =>
            {
                var person = _personService.GetByUserId(f.FollowingUserId);
                return new FollowerDto
                    {
                        Id = f.Id,
                        UserId = f.FollowingUserId,
                        Name = $"{person.Name} {person.Surname}",
                        FollowedAt = f.FollowedAt
                    };
            }).ToList();
    }

    public void SendMessageToFollowers(long senderId, SendFollowerMessageDto messageDto)
    {
        // Parse attachment type
        ResourceType? attachmentType = null;
        if (!string.IsNullOrWhiteSpace(messageDto.AttachmentType))
        {
            if (!Enum.TryParse<ResourceType>(messageDto.AttachmentType, true, out var parsed))
                throw new ArgumentException($"Invalid attachment type: {messageDto.AttachmentType}");
            attachmentType = parsed;
        }

        // Create message
        var message = new FollowerMessage(
            senderId,
            messageDto.Content,
            attachmentType,
            messageDto.AttachmentResourceId
        );

        var createdMessage = _messageRepository.Create(message);

        // Get all followers
        var followers = _followerRepository.GetFollowersByUserId(senderId);

        // Create notifications for each follower
        var senderPerson = _personService.GetByUserId(senderId);
        foreach (var follower in followers)
        {
            var notification = new Notification(
                follower.UserId,
                NotificationType.FollowerMessage,
                $"New message from {senderPerson.Name}",
                messageDto.Content,
                createdMessage.Id,
                attachmentType.HasValue ? $"{attachmentType}:{messageDto.AttachmentResourceId}" : null
            );

            _notificationRepository.Create(notification);
        }
    }

    public List<NotificationDto> GetNotifications(long userId)
    {
        var notifications = _notificationRepository.GetByUserId(userId);
        return notifications.Select(_mapper.Map<NotificationDto>).ToList();
    }

    public List<NotificationDto> GetUnreadNotifications(long userId)
    {
        var notifications = _notificationRepository.GetUnreadByUserId(userId);
        return notifications.Select(_mapper.Map<NotificationDto>).ToList();
    }

    public void MarkNotificationAsRead(long notificationId)
    {
        var notification = _notificationRepository.Get(notificationId);
        if (notification == null)
            throw new KeyNotFoundException("Notification not found.");

        notification.MarkAsRead();
        _notificationRepository.Update(notification);
    }
}
