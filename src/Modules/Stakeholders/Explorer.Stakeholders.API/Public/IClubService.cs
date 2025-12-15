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
        void Invite(long clubId, long ownerId, long touristId);
        void Expel(long clubId, long ownerId, long touristId);
        void Close(long clubId, long ownerId);
        void Activate(long clubId, long ownerId);
    }
}
