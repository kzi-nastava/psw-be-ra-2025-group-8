using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IRatingService
    {
        RatingDto Create(RatingNoIdDto rating, long userId);
        PagedResult<RatingDto> GetPaged(int page, int pageSize);
        RatingDto GetByUserId(long userId);
        RatingDto Get(int id);
        RatingDto UpdateByUserId(RatingNoIdDto rating, long userId);
        RatingDto Update(RatingDto rating, long userId);
        void DeleteByUserId(long userId);
        void Delete(int id, long userId);
    }
}