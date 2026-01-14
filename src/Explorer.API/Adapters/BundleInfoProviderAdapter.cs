using System.Linq;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.Core.UseCases;
using Explorer.Tours.API.Public.Tourist;

namespace Explorer.API.Adapters
{
    public class BundleInfoProviderAdapter : IBundleInfoProvider
    {
        private readonly ITouristBundleService _bundleService;

        public BundleInfoProviderAdapter(ITouristBundleService bundleService)
        {
            _bundleService = bundleService;
        }

        public BundleInfoDto? GetPublishedById(long id)
        {
            try
            {
                var bundle = _bundleService.GetPublishedById(id);
                return new BundleInfoDto
                {
                    Id = bundle.Id,
                    Price = bundle.Price,
                    TourIds = bundle.Tours.Select(t => t.Id).ToList()
                };
            }
            catch (NotFoundException)
            {
                return null;
            }
        }
    }
}

