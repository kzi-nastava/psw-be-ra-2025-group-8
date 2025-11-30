using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.ShoppingCart
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetCart(long userId);
        void AddItem(long userId, OrderItemDto item);
        void RemoveItem(long userId, long tourId);
        void ClearCart(long userId);
    }
}
