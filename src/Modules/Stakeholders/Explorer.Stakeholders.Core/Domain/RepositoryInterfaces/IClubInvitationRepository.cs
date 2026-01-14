using System.Collections.Generic;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubInvitationRepository
    {
        ClubInvitation Create(ClubInvitation invitation);
        ClubInvitation Get(long id);
        ClubInvitation Update(ClubInvitation invitation);
        void Delete(long id);
        IEnumerable<ClubInvitation> GetByClubId(long clubId);
        IEnumerable<ClubInvitation> GetByTouristId(long touristId);
        ClubInvitation? GetPendingInvitation(long clubId, long touristId);
    }
}
