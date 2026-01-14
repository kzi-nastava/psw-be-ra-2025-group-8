using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Payments.Core.UseCases
{
    public class BundlePurchaseService : IBundlePurchaseService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IBundlePurchaseRecordRepository _bundlePurchaseRecordRepository;
        private readonly IBundleInfoProvider _bundleInfoProvider;
        private readonly IInternalWalletService _walletService;
        private readonly IPurchaseNotificationService _purchaseNotificationService;
        private readonly IMapper _mapper;

        public BundlePurchaseService(
            IShoppingCartRepository cartRepository,
            IBundlePurchaseRecordRepository bundlePurchaseRecordRepository,
            IBundleInfoProvider bundleInfoProvider,
            IInternalWalletService walletService,
            IPurchaseNotificationService purchaseNotificationService,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _bundlePurchaseRecordRepository = bundlePurchaseRecordRepository;
            _bundleInfoProvider = bundleInfoProvider;
            _walletService = walletService;
            _purchaseNotificationService = purchaseNotificationService;
            _mapper = mapper;
        }

        public BundlePurchaseRecordDto PurchasePublishedBundle(long touristUserId, long bundleId)
        {
            var bundle = _bundleInfoProvider.GetPublishedById(bundleId);
            if (bundle == null) throw new NotFoundException("Bundle not found.");

            int requiredCoins = (int)Math.Ceiling(bundle.Price);

            if (!_walletService.HasSufficientFunds(touristUserId, requiredCoins))
                throw new InvalidOperationException($"Insufficie...ins. Required: {requiredCoins}, but user doesn't have enough.");

            _walletService.DeductCoins(touristUserId, requiredCoins);

            // Treba nam ShoppingCart row da bismo upisali PurchasedItem tokene
            var cart = _cartRepository.GetByUserId(touristUserId);
            if (cart == null)
            {
                cart = new ShoppingCart(touristUserId);
                _cartRepository.Add(cart);
            }

            // Za svaku turu iz bundle-a upiši token (PurchasedItem)
            foreach (var tourId in bundle.TourIds)
            {
                if (cart.PurchasedItems.Any(pi => pi.TourId == tourId)) continue; // idempotentno
                cart.RecordDirectPurchase(tourId, 0m, 0); // cena/coins 0 jer je plaćeno kroz bundle
            }

            _cartRepository.Update(cart);

            // Payment record za bundle (1 zapis)
            var record = new BundlePurchaseRecord(touristUserId, bundleId, bundle.Price, requiredCoins);
            _bundlePurchaseRecordRepository.Add(record);

            _purchaseNotificationService.NotifyToursPurchased(touristUserId, bundle.TourIds);

            var dto = _mapper.Map<BundlePurchaseRecordDto>(record);
            dto.TourIds = bundle.TourIds;
            return dto;
        }
    }
}

