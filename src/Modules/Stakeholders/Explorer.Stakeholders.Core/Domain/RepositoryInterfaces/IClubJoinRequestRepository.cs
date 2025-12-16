using System.Collections.Generic;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubJoinRequestRepository
    {
        ClubJoinRequest Create(ClubJoinRequest request);
        ClubJoinRequest Get(long id);
        ClubJoinRequest Update(ClubJoinRequest request);
        void Delete(long id);
        IEnumerable<ClubJoinRequest> GetByClubId(long clubId);
        IEnumerable<ClubJoinRequest> GetByTouristId(long touristId);
        ClubJoinRequest? GetPendingRequest(long clubId, long touristId);
    }
}
