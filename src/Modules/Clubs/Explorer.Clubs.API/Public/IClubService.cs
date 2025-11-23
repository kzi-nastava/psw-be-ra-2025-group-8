using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Clubs.API.Dtos;

namespace Explorer.Clubs.API.Public
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
    }
}
