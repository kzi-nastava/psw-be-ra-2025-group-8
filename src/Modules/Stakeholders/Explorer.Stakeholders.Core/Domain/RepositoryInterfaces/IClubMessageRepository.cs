using System.Collections.Generic;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubMessageRepository
    {
        ClubMessage Create(ClubMessage message);
        ClubMessage Get(long id);
        IEnumerable<ClubMessage> GetByClubId(long clubId);
        ClubMessage Update(ClubMessage message);
        void Delete(long id);
    }
}
