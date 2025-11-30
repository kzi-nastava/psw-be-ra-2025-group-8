using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IShoppingCartRepository
    {
        ShoppingCart GetById(long id);
        ShoppingCart GetByUserId(long userId);
        void Add(ShoppingCart cart);
        void Update(ShoppingCart cart);
        void Delete(long id);
    }
}
