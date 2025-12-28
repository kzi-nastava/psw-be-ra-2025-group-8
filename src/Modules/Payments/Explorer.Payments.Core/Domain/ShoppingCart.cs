using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class ShoppingCart : AggregateRoot
    {
        private List<OrderItem> _items = new();
        private List<PurchasedItem> _purchasedItems = new();
        
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public IReadOnlyCollection<PurchasedItem> PurchasedItems => _purchasedItems.AsReadOnly();
        public long UserId { get; private set; }

        // EF Core constructor
        private ShoppingCart() { }

        public ShoppingCart(long userId)
        {
            //ovaj if statement je zasad zakomentiran jer ne znam koji ce format imati user Id
            //if (userId <= 0) throw new ArgumentException("Invalid user ID");
            UserId = userId;
        }

        public void AddItem(OrderItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // sprečava duplikate tura
            if (_items.Any(i => i.TourId == item.TourId))
                throw new InvalidOperationException("Tour already in cart.");

            _items.Add(item);
        }

        public void RemoveItem(long tourId)
        {
            var item = _items.FirstOrDefault(i => i.TourId == tourId);
            if (item == null) throw new KeyNotFoundException("Item not found in cart.");
            _items.Remove(item);
        }

        public void ClearCart()
        {
            _items.Clear();
        }

        public void PurchaseItem(long tourId, decimal price)
        {
            var item = _items.FirstOrDefault(i => i.TourId == tourId);
            if (item == null) throw new KeyNotFoundException("Item not found in cart.");

            int adventureCoinsSpent = (int)Math.Ceiling(price);
            var purchasedItem = UserId > 0 
                ? new PurchasedItem(UserId, tourId, price, adventureCoinsSpent)
                : new PurchasedItem(tourId, price); // Fallback for backward compatibility
            
            _purchasedItems.Add(purchasedItem);
            _items.Remove(item);
        }

        public void PurchaseAllItems(Dictionary<long, decimal> tourPrices)
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("Cart is empty.");

            var itemsToRemove = _items.ToList();
            
            foreach (var item in itemsToRemove)
            {
                if (!tourPrices.ContainsKey(item.TourId))
                    throw new KeyNotFoundException($"Price not found for tour {item.TourId}");

                int adventureCoinsSpent = (int)Math.Ceiling(tourPrices[item.TourId]);
                var purchasedItem = UserId > 0
                    ? new PurchasedItem(UserId, item.TourId, tourPrices[item.TourId], adventureCoinsSpent)
                    : new PurchasedItem(item.TourId, tourPrices[item.TourId]); // Fallback for backward compatibility
                
                _purchasedItems.Add(purchasedItem);
            }

            _items.Clear();
        }
    }
}
