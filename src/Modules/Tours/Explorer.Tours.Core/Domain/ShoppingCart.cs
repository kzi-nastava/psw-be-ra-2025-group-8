using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class ShoppingCart : AggregateRoot
    {
        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
        public long UserId { get; private set; }    
        public decimal TotalPrice => _items.Sum(i => i.Price);

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
    }
}
