using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubService
    {
        ClubDto Create(CreateClubDto dto, long ownerId);
        ClubDto Get(long id);
        IEnumerable<ClubDto> GetAll();
        void Join(long Id, long touristId);
        ClubDto Update(long id, long current_owner_id, ClubDto entity, long userid);
        void Delete(long ownerId, long Id);
        PagedResult<ClubDto> GetPaged(int page, int pageSize);

        // owner actions
        void Expel(long clubId, long ownerId, long touristId);
        void Close(long clubId, long ownerId);
        void Activate(long clubId, long ownerId);

        // invitations (owner invites tourist by username)
        ClubInvitationDto InviteTouristByUsername(long clubId, long ownerId, string username);
        IEnumerable<ClubInvitationDto> GetMyInvitations(long touristId);
        IEnumerable<ClubInvitationDto> GetClubInvitations(long clubId, long ownerId);
        ClubInvitationDto AcceptInvitation(long invitationId, long touristId);
        ClubInvitationDto RejectInvitation(long invitationId, long touristId);
        void CancelInvitation(long invitationId, long ownerId);

        // join requests (tourist requests to join)
        ClubJoinRequestDto RequestToJoin(long clubId, long touristId);
        void CancelJoinRequest(long requestId, long touristId);
        ClubJoinRequestDto AcceptJoinRequest(long requestId, long ownerId);
        ClubJoinRequestDto RejectJoinRequest(long requestId, long ownerId);
        IEnumerable<ClubJoinRequestDto> GetClubJoinRequests(long clubId, long ownerId);
        IEnumerable<ClubJoinRequestDto> GetMyJoinRequests(long touristId);
    }
}
