namespace Explorer.Payments.Core.UseCases;

public class TourPriceDto
{
    public long Id { get; set; }
    public decimal Price { get; set; }
}

public interface ITourPriceProvider
{
    // returns null when tour not found
    TourPriceDto? GetById(long id);
}