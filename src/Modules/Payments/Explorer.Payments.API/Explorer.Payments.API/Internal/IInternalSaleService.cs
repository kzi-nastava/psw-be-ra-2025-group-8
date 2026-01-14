using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Internal
{
    public interface IInternalSaleService
    {
        TourSaleInfoDto GetTourSaleInfo(long tourId);
        List<TourSaleInfoDto> GetToursSaleInfo(List<long> tourIds);
        List<SaleDto> GetActiveSales();
    }
}
