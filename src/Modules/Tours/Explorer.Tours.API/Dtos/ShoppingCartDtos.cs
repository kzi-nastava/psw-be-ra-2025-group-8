namespace Explorer.Tours.API.Dtos
{
    public class OrderItemDto
    {
        public long TourId { get; set; }
    }

    public class PurchasedItemDto
    {
        public long Id { get; set; }
        public long TourId { get; set; }
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
    }

    public class ShoppingCartDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
        public List<PurchasedItemDto> PurchasedItems { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}
