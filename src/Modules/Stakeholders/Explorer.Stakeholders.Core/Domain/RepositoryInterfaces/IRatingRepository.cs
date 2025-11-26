using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IRatingRepository: ICrudRepository<Rating>
    {
        //ICrudRepository sadrzi CRUD
        Rating GetByUserId(long userId);
    }
}