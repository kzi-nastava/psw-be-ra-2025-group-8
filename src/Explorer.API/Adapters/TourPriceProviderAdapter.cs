using Explorer.Payments.Core.UseCases;
using Explorer.Tours.API.Public.Author;

namespace Explorer.API.Adapters;

public class TourPriceProviderAdapter : ITourPriceProvider
{
    private readonly ITourService _tourService;

    public TourPriceProviderAdapter(ITourService tourService)
    {
        _tourService = tourService;
    }

    public Explorer.Payments.Core.UseCases.TourPriceDto? GetById(long id)
    {
        var tour = _tourService.GetById(id);
        if (tour == null) return null;
        return new Explorer.Payments.Core.UseCases.TourPriceDto
        {
            Id = tour.Id,
            Price = tour.Price,
            AuthorId = tour.AuthorId
        };
    }
}

