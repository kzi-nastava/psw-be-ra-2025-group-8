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
        private readonly IClubJoinRequestRepository _joinRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository repo, IClubJoinRequestRepository joinRequestRepository, 
            INotificationService notificationService, IMapper mapper)
        {
            _clubRepository = repo;
            _joinRequestRepository = joinRequestRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public PagedResult<ClubDto> GetPaged(int page, int pageSize)
        {
            var result = _clubRepository.GetPaged(page, pageSize);

            var items = result.Results.Select(_mapper.Map<ClubDto>).ToList();
            return new PagedResult<ClubDto>(items, result.TotalCount);
        }
        public ClubDto Create(CreateClubDto dto, long ownerId)
        {
            var club = new Club(ownerId, dto.Name, dto.Description, dto.ImageUrls);
            var created = _clubRepository.Create(club);
            return _mapper.Map<ClubDto>(created);
        }
        public ClubDto Get(long id)
        {
            var club = _clubRepository.Get(id);
            return _mapper.Map<ClubDto>(club);
        }
        public IEnumerable<ClubDto> GetAll()
        {
            var clubs = _clubRepository.GetAll();
            return _mapper.Map<IEnumerable<ClubDto>>(clubs);
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

            return _mapper.Map<ClubDto>(updated);
        }


        public void Delete(long userId, long id)
        {
            var club = _clubRepository.Get(id);
            if (club.OwnerId != userId)
                throw new UnauthorizedAccessException("Only the owner can delete the club");
            _clubRepository.Delete(id);
        }

        // owner actions
        public void Invite(long clubId, long ownerId, long touristId)
        {
            var club = _clubRepository.Get(clubId);
            club.InviteMember(ownerId, touristId);
            _clubRepository.Update(club);
        }

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
            
            return _mapper.Map<ClubJoinRequestDto>(created);
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

            return _mapper.Map<ClubJoinRequestDto>(request);
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

            return _mapper.Map<ClubJoinRequestDto>(request);
        }

        public IEnumerable<ClubJoinRequestDto> GetClubJoinRequests(long clubId, long ownerId)
        {
            var club = _clubRepository.Get(clubId);
            
            if (!club.IsOwner(ownerId))
                throw new UnauthorizedAccessException("Only the club owner can view join requests.");

            var requests = _joinRequestRepository.GetByClubId(clubId)
                .Where(r => r.Status == ClubJoinRequestStatus.Pending);
            
            return _mapper.Map<IEnumerable<ClubJoinRequestDto>>(requests);
        }

        public IEnumerable<ClubJoinRequestDto> GetMyJoinRequests(long touristId)
        {
            var requests = _joinRequestRepository.GetByTouristId(touristId)
                .Where(r => r.Status == ClubJoinRequestStatus.Pending);
            
            return _mapper.Map<IEnumerable<ClubJoinRequestDto>>(requests);
        }
    }
}
