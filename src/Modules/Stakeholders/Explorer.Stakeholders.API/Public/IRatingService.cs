using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IRatingService
    {
        RatingDto Create(RatingDto rating, long userId);

        PagedResult<RatingDto> GetPaged(int page, int pageSize);

        RatingDto Get(int id);

        RatingDto Update(RatingDto rating, long userId);

        void Delete(int id, long userId);
    }
}