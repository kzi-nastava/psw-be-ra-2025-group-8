using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface IShoppingCartService
    {
        ShoppingCartDto CreateCart(long userId);
        ShoppingCartDto GetCart(long userId);
        List<PurchasedItemDto> GetPurchasedItems(long userId);
        void AddItem(long userId, OrderItemDto item);
        void RemoveItem(long userId, long tourId);
        void ClearCart(long userId);
        void DeleteCart(long userId);
        void PurchaseItem(long userId, long tourId);
        void PurchaseAllItems(long userId);
        void PurchaseItemWithCoupon(long userId, long tourId, string couponCode);
        void PurchaseAllItemsWithCoupon(long userId, string couponCode);
    }
}
