using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class SaleService : ISaleService, IInternalSaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ITourPriceProvider _tourPriceProvider;
        private readonly IMapper _mapper;

        public SaleService(ISaleRepository saleRepository, ITourPriceProvider tourPriceProvider, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _tourPriceProvider = tourPriceProvider;
            _mapper = mapper;
        }

        public SaleDto Create(CreateSaleDto dto, long authorId)
        {
            // Verify that all tours belong to the author
            foreach (var tourId in dto.TourIds)
            {
                var tour = _tourPriceProvider.GetById(tourId);
                if (tour == null)
                    throw new KeyNotFoundException($"Tour with ID {tourId} not found.");
                
                if (tour.AuthorId != authorId)
                    throw new UnauthorizedAccessException($"Tour with ID {tourId} does not belong to this author.");
            }

            var sale = new Sale(authorId, dto.StartDate, dto.EndDate, dto.DiscountPercentage, dto.TourIds);
            var created = _saleRepository.Create(sale);
            return _mapper.Map<SaleDto>(created);
        }

        public SaleDto Update(long saleId, UpdateSaleDto dto, long authorId)
        {
            var sale = _saleRepository.GetByIdAndAuthor(saleId, authorId);
            if (sale == null)
                throw new KeyNotFoundException("Sale not found or you don't have permission to modify it.");

            // Verify that all tours belong to the author
            foreach (var tourId in dto.TourIds)
            {
                var tour = _tourPriceProvider.GetById(tourId);
                if (tour == null)
                    throw new KeyNotFoundException($"Tour with ID {tourId} not found.");
                
                if (tour.AuthorId != authorId)
                    throw new UnauthorizedAccessException($"Tour with ID {tourId} does not belong to this author.");
            }

            sale.Update(dto.StartDate, dto.EndDate, dto.DiscountPercentage, dto.TourIds);
            var updated = _saleRepository.Update(sale);
            return _mapper.Map<SaleDto>(updated);
        }

        public void Delete(long saleId, long authorId)
        {
            var sale = _saleRepository.GetByIdAndAuthor(saleId, authorId);
            if (sale == null)
                throw new KeyNotFoundException("Sale not found or you don't have permission to delete it.");

            _saleRepository.Delete(saleId);
        }

        public SaleDto Get(long saleId)
        {
            var sale = _saleRepository.Get(saleId);
            if (sale == null)
                throw new KeyNotFoundException("Sale not found.");

            return _mapper.Map<SaleDto>(sale);
        }

        public List<SaleDto> GetByAuthor(long authorId)
        {
            var sales = _saleRepository.GetByAuthor(authorId);
            return _mapper.Map<List<SaleDto>>(sales);
        }

        public List<SaleDto> GetActiveSales()
        {
            var sales = _saleRepository.GetActiveSales();
            return _mapper.Map<List<SaleDto>>(sales);
        }

        public PagedResult<SaleDto> GetPaged(int page, int pageSize)
        {
            var result = _saleRepository.GetPaged(page, pageSize);
            var dtos = _mapper.Map<List<SaleDto>>(result.Results);
            return new PagedResult<SaleDto>(dtos, result.TotalCount);
        }

        public TourSaleInfoDto GetTourSaleInfo(long tourId)
        {
            var tour = _tourPriceProvider.GetById(tourId);
            if (tour == null)
                throw new KeyNotFoundException($"Tour with ID {tourId} not found.");

            var activeSales = _saleRepository.GetActiveSalesForTour(tourId);
            
            // Get the best discount if multiple sales apply
            var bestSale = activeSales
                .Where(s => s.IsActive() && s.AppliesTo(tourId))
                .OrderByDescending(s => s.DiscountPercentage)
                .FirstOrDefault();

            if (bestSale == null)
            {
                return new TourSaleInfoDto
                {
                    TourId = tourId,
                    IsOnSale = false,
                    OriginalPrice = tour.Price
                };
            }

            return new TourSaleInfoDto
            {
                TourId = tourId,
                IsOnSale = true,
                DiscountPercentage = bestSale.DiscountPercentage,
                OriginalPrice = tour.Price,
                DiscountedPrice = bestSale.CalculateDiscountedPrice(tour.Price)
            };
        }

        public List<TourSaleInfoDto> GetToursSaleInfo(List<long> tourIds)
        {
            return tourIds.Select(GetTourSaleInfo).ToList();
        }
    }
}
