using System.Collections.Generic;
using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IClubJoinRequestRepository _joinRequestRepository;
        private readonly IClubInvitationRepository _invitationRepository;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ClubService(
            IClubRepository repo, 
            IClubJoinRequestRepository joinRequestRepository,
            IClubInvitationRepository invitationRepository,
            INotificationService notificationService, 
            IUserRepository userRepository,
            IMapper mapper, 
            INotificationRepository notificationRepository)
        {
            _clubRepository = repo;
            _joinRequestRepository = joinRequestRepository;
            _invitationRepository = invitationRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _mapper = mapper;
            _notificationRepository = notificationRepository;
        }

        public PagedResult<ClubDto> GetPaged(int page, int pageSize)
        {
            var result = _clubRepository.GetPaged(page, pageSize);

            var items = result.Results
                .Select(_mapper.Map<ClubDto>)
                .Select(EnrichClubWithUsernames)
                .ToList();
            return new PagedResult<ClubDto>(items, result.TotalCount);
        }
        public ClubDto Create(CreateClubDto dto, long ownerId)
        {
            var club = new Club(ownerId, dto.Name, dto.Description, dto.ImageUrls);
            var created = _clubRepository.Create(club);
            var clubDto = _mapper.Map<ClubDto>(created);
            return EnrichClubWithUsernames(clubDto);
        }
        public ClubDto Get(long id)
        {
            var club = _clubRepository.Get(id);
            var dto = _mapper.Map<ClubDto>(club);
            return EnrichClubWithUsernames(dto);
        }
        public IEnumerable<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            var dtos = _mapper.Map<IEnumerable<ClubDto>>(clubs);
            return dtos.Select(EnrichClubWithUsernames);
        }
        public void Join(long id, long touristId)
        {
            var club = _clubRepository.Get(id);
            club.AddMember(touristId);
            _clubRepository.Update(club);

            // Create notification for club owner about new member
            var notification = new Notification(
                club.OwnerId,
                NotificationType.ClubActivity,
                $"New member joined {club.Name}",
                $"User {touristId} has joined your club",
                id,
                $"Club:{id}"
            );
            _notificationRepository.Create(notification);
        }
        public ClubDto Update(long id, long current_owner_id, ClubDto dto, long user_id)
        {
            if (user_id != current_owner_id)
                throw new UnauthorizedAccessException("Only the owner can update the club");

            // 1. fetch postojeći klub
            var existing = _clubRepository.Get(id);

            // 2. update polja
            existing.Update(dto.Name, dto.Description, dto.ImageUrls);

            // 3. sačuvaj
            var updated = _clubRepository.Update(existing);

            var updatedDto = _mapper.Map<ClubDto>(updated);
            return EnrichClubWithUsernames(updatedDto);
        }


        public void Delete(long userId, long id)
        {
            var club = _clubRepository.Get(id);
            if (club.OwnerId != userId)
                throw new UnauthorizedAccessException("Only the owner can delete the club");
            _clubRepository.Delete(id);
        }

        // owner actions
        public void Expel(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            club.ExpelMember(ownerId, touristId);
            _clubRepository.Update(club);
        }

        public void Close(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the owner can close the club.");
            club.Close();
            _clubRepository.Update(club);
        }

        public void Activate(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the owner can activate the club.");
            club.Activate();
            _clubRepository.Update(club);
        }

        // Invitation Methods (Owner invites Tourist)
        public ClubInvitationDto InviteTouristByUsername(long clubId, long ownerId, string username)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can send invitations.");

            if (club.Status != ClubStatus.Active)
                throw new InvalidOperationException("Cannot send invitations when the club is not active.");

            // Find tourist by username
            var user = _userRepository.GetActiveByName(username);
            if (user == null)
                throw new KeyNotFoundException($"User with username '{username}' not found.");

            if (user.Role != UserRole.Tourist)
                throw new InvalidOperationException("You can only invite tourists to the club.");

            var touristUserId = user.Id; // Use UserId instead of PersonId

            // Check if already a member
            if (club.MemberIds.Contains(touristUserId))
                throw new InvalidOperationException("This user is already a member of the club.");

            // Check if there's already a pending invitation
            var existingInvitation = _invitationRepository.GetPendingInvitation(clubId, touristUserId);
            if (existingInvitation != null)
                throw new InvalidOperationException("This user already has a pending invitation.");

            // Create invitation
            var invitation = new ClubInvitation(clubId, touristUserId);
            var created = _invitationRepository.Create(invitation);

            // Send notification to tourist
            _notificationService.Create(new NotificationDto
            {
                UserId = touristUserId,
                Type = 1,
                Title = "Club Invitation",
                Content = $"You have been invited to join club '{club.Name}'",
                RelatedEntityId = clubId,
                RelatedEntityType = "Club"
            });

            var invitationDto = _mapper.Map<ClubInvitationDto>(created);
            return EnrichInvitationWithUsername(invitationDto);
        }

        public IEnumerable<ClubInvitationDto> GetMyInvitations(long touristId)
        {
            var invitations = _invitationRepository.GetByTouristId(touristId)
                .Where(i => i.Status == ClubInvitationStatus.Pending);
            
            var dtos = _mapper.Map<IEnumerable<ClubInvitationDto>>(invitations);
            return dtos.Select(EnrichInvitationWithUsername);
        }

        public IEnumerable<ClubInvitationDto> GetClubInvitations(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can view invitations.");

            var invitations = _invitationRepository.GetByClubId(clubId)
                .Where(i => i.Status == ClubInvitationStatus.Pending);
            
            var dtos = _mapper.Map<IEnumerable<ClubInvitationDto>>(invitations);
            return dtos.Select(EnrichInvitationWithUsername);
        }

        public ClubInvitationDto AcceptInvitation(long invitationId, long touristId)
        {
            var invitation = _invitationRepository.Get(invitationId);
            
            if (invitation.TouristId != touristId)
                throw new UnauthorizedAccessException("You can only accept your own invitations.");

            if (invitation.Status != ClubInvitationStatus.Pending)
                throw new InvalidOperationException("This invitation is no longer pending.");

            var club = _clubRepository.Get(invitation.ClubId);
            
            // Accept invitation
            invitation.Accept();
            club.AddMember(touristId);
            
            _clubRepository.Update(club);
            _invitationRepository.Delete(invitationId);

            // Notify club owner
            _notificationService.Create(new NotificationDto
            {
                UserId = club.OwnerId,
                Type = 1,
                Title = "Invitation Accepted",
                Content = $"A tourist has accepted your invitation to join '{club.Name}'",
                RelatedEntityId = club.Id,
                RelatedEntityType = "Club"
            });

            var invitationDto = _mapper.Map<ClubInvitationDto>(invitation);
            return EnrichInvitationWithUsername(invitationDto);
        }

        public ClubInvitationDto RejectInvitation(long invitationId, long touristId)
        {
            var invitation = _invitationRepository.Get(invitationId);
            
            if (invitation.TouristId != touristId)
                throw new UnauthorizedAccessException("You can only reject your own invitations.");

            if (invitation.Status != ClubInvitationStatus.Pending)
                throw new InvalidOperationException("This invitation is no longer pending.");

            var club = _clubRepository.Get(invitation.ClubId);
            
            invitation.Reject();
            _invitationRepository.Delete(invitationId);

            // Notify club owner
            _notificationService.Create(new NotificationDto
            {
                UserId = club.OwnerId,
                Type = 1,
                Title = "Invitation Rejected",
                Content = $"A tourist has rejected your invitation to join '{club.Name}'",
                RelatedEntityId = club.Id,
                RelatedEntityType = "Club"
            });

            var invitationDto = _mapper.Map<ClubInvitationDto>(invitation);
            return EnrichInvitationWithUsername(invitationDto);
        }

        public void CancelInvitation(long invitationId, long ownerId)
        {
            var invitation = _invitationRepository.Get(invitationId);
            var club = _clubRepository.Get(invitation.ClubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can cancel invitations.");

            if (invitation.Status != ClubInvitationStatus.Pending)
                throw new InvalidOperationException("This invitation is no longer pending.");

            invitation.Cancel();
            _invitationRepository.Delete(invitationId);
        }

        // Join Request Methods
        public ClubJoinRequestDto RequestToJoin(long clubId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (club.Status != ClubStatus.Active)
                throw new InvalidOperationException("Cannot request to join a closed club.");

            if (club.MemberIds.Contains(touristId))
                throw new InvalidOperationException("You are already a member of this club.");

            var existingRequest = _joinRequestRepository.GetPendingRequest(clubId, touristId);
            if (existingRequest != null)
                throw new InvalidOperationException("You already have a pending join request for this club.");

            var request = new ClubJoinRequest(clubId, touristId);
            var created = _joinRequestRepository.Create(request);
            
            var requestDto = _mapper.Map<ClubJoinRequestDto>(created);
            return EnrichJoinRequestWithUsername(requestDto);
        }

        public void CancelJoinRequest(long requestId, long touristId)
        {
            var request = _joinRequestRepository.Get(requestId);
            
            if (request.TouristId != touristId)
                throw new UnauthorizedAccessException("You can only cancel your own requests.");

            request.Cancel();
            _joinRequestRepository.Delete(requestId);
        }

        public ClubJoinRequestDto AcceptJoinRequest(long requestId, long ownerId)
        {
            var request = _joinRequestRepository.Get(requestId);
            var club = _clubRepository.Get(request.ClubId);

            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can accept join requests.");

            request.Accept();
            club.AddMember(request.TouristId);
            
            _clubRepository.Update(club);
            _joinRequestRepository.Delete(requestId);

            _notificationService.Create(new NotificationDto
            {
                UserId = request.TouristId,
                Type = 1,
                Title = "Join Request Accepted",
                Content = $"Your request to join club '{club.Name}' has been accepted!",
                RelatedEntityId = club.Id,
                RelatedEntityType = "Club"
            });

            var requestDto = _mapper.Map<ClubJoinRequestDto>(request);
            return EnrichJoinRequestWithUsername(requestDto);
        }

        public ClubJoinRequestDto RejectJoinRequest(long requestId, long ownerId)
        {
            var request = _joinRequestRepository.Get(requestId);
            var club = _clubRepository.Get(request.ClubId);

            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can reject join requests.");

            request.Reject();
            _joinRequestRepository.Delete(requestId);

            _notificationService.Create(new NotificationDto
            {
                UserId = request.TouristId,
                Type = 1,
                Title = "Join Request Rejected",
                Content = $"Your request to join club '{club.Name}' has been rejected.",
                RelatedEntityId = club.Id,
                RelatedEntityType = "Club"
            });

            var requestDto = _mapper.Map<ClubJoinRequestDto>(request);
            return EnrichJoinRequestWithUsername(requestDto);
        }

        public IEnumerable<ClubJoinRequestDto> GetClubJoinRequests(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can view join requests.");

            var requests = _joinRequestRepository.GetByClubId(clubId)
                .Where(r => r.Status == ClubJoinRequestStatus.Pending);
            
            var dtos = _mapper.Map<IEnumerable<ClubJoinRequestDto>>(requests);
            return dtos.Select(EnrichJoinRequestWithUsername);
        }

        public IEnumerable<ClubJoinRequestDto> GetMyJoinRequests(long touristId)
        {
            var requests = _joinRequestRepository.GetByTouristId(touristId)
                .Where(r => r.Status == ClubJoinRequestStatus.Pending);
            
            var dtos = _mapper.Map<IEnumerable<ClubJoinRequestDto>>(requests);
            return dtos.Select(EnrichJoinRequestWithUsername);
        }

        private ClubDto EnrichClubWithUsernames(ClubDto dto)
        {
            dto.OwnerUsername = GetUsernameById(dto.OwnerId);
            dto.MemberUsernames = dto.MemberIds.Select(GetUsernameById).ToList();
            return dto;
        }

        private ClubJoinRequestDto EnrichJoinRequestWithUsername(ClubJoinRequestDto dto)
        {
            dto.TouristUsername = GetUsernameById(dto.TouristId);
            return dto;
        }

        private ClubInvitationDto EnrichInvitationWithUsername(ClubInvitationDto dto)
        {
            dto.TouristUsername = GetUsernameById(dto.TouristId);
            return dto;
        }

        private string GetUsernameById(long userId)
        {
            var user = _userRepository.GetById(userId);
            return user?.Username ?? $"User#{userId}";
        }
    }
}
