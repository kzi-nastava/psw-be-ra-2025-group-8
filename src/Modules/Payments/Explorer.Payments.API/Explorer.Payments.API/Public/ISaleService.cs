using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface ISaleService
    {
        SaleDto Create(CreateSaleDto dto, long authorId);
        SaleDto Update(long saleId, UpdateSaleDto dto, long authorId);
        void Delete(long saleId, long authorId);
        SaleDto Get(long saleId);
        List<SaleDto> GetByAuthor(long authorId);
        List<SaleDto> GetActiveSales();
        PagedResult<SaleDto> GetPaged(int page, int pageSize);
        TourSaleInfoDto GetTourSaleInfo(long tourId);
        List<TourSaleInfoDto> GetToursSaleInfo(List<long> tourIds);
    }
}
