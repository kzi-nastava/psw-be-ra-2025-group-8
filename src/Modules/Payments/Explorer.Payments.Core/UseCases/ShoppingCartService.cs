using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Internal;
using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Payments.Core.UseCases
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourPriceProvider _tourPriceProvider;
        private readonly IInternalWalletService _walletService;
        private readonly IMapper _mapper;
        private readonly IPurchaseNotificationService _purchaseNotificationService;

        public ShoppingCartService(
            IShoppingCartRepository cartRepository, 
            ITourPriceProvider tourPriceProvider, 
            IInternalWalletService walletService,
            IMapper mapper,
            IPurchaseNotificationService purchaseNotificationService)
        {
            _cartRepository = cartRepository;
            _tourPriceProvider = tourPriceProvider;
            _walletService = walletService;
            _mapper = mapper;
            _purchaseNotificationService = purchaseNotificationService;
        }

        public ShoppingCartDto CreateCart(long userId)
        {
            var existingCart = _cartRepository.GetByUserId(userId);
            if (existingCart != null)
            {
                throw new InvalidOperationException("Cart already exists for this user.");
            }
            var cart = new Domain.ShoppingCart(userId);
            _cartRepository.Add(cart);
            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public ShoppingCartDto GetCart(long userId)
        {
            var cart = _cartRepository.GetByUserId(userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found for this user.");
            }
            var dto = _mapper.Map<ShoppingCartDto>(cart);
            dto.TotalPrice = CalculateTotalPrice(cart);
            return dto;
        }

        public List<PurchasedItemDto> GetPurchasedItems(long userId)
        {
            var cart = _cartRepository.GetByUserId(userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found for this user.");
            }

            return _mapper.Map<List<PurchasedItemDto>>(cart.PurchasedItems);
        }

        public void AddItem(long userId, OrderItemDto itemDto)
        {
            var cart = _cartRepository.GetByUserId(userId);

            if (cart == null)
            {
                throw new NotFoundException("Cart not found for this user.");
            }

            var item = _mapper.Map<OrderItem>(itemDto);
            cart.AddItem(item);

            _cartRepository.Update(cart);
        }

        public void RemoveItem(long userId, long tourId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null) throw new KeyNotFoundException("Cart not found for this user.");

            cart.RemoveItem(tourId);
            _cartRepository.Update(cart);
        }

        public void ClearCart(long userId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null) throw new KeyNotFoundException("Cart not found for this user.");

            cart.ClearCart();
            _cartRepository.Update(cart);
        }

        public void DeleteCart(long userId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null) throw new KeyNotFoundException("Cart not found for this user.");
            _cartRepository.Delete(cart.Id);
        }

        public void PurchaseItem(long userId, long tourId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null) throw new NotFoundException("Cart not found for this user.");

            var tour = _tourPriceProvider.GetById(tourId);
            if (tour == null) throw new NotFoundException("Tour not found.");

            int requiredCoins = (int)Math.Ceiling(tour.Price);

            // Check if user has sufficient Adventure Coins
            if (!_walletService.HasSufficientFunds(userId, requiredCoins))
                throw new InvalidOperationException($"Insufficient Adventure Coins. Required: {requiredCoins}, but user doesn't have enough.");

            // Deduct coins from wallet
            _walletService.DeductCoins(userId, requiredCoins);

            // Record purchase
            cart.PurchaseItem(tourId, tour.Price);
            _cartRepository.Update(cart);
            _purchaseNotificationService.NotifyTourPurchased(userId, tourId);
        }

        public void PurchaseAllItems(long userId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null) throw new NotFoundException("Cart not found for this user.");

            if (!cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var tourPrices = new Dictionary<long, decimal>();
            int totalRequiredCoins = 0;
            var purchasedTourIds = cart.Items.Select(i => i.TourId).ToList();

            foreach (var item in cart.Items)
            {
                var tour = _tourPriceProvider.GetById(item.TourId);
                if (tour == null)
                    throw new NotFoundException($"Tour with ID {item.TourId} not found.");

                tourPrices[item.TourId] = tour.Price;
                totalRequiredCoins += (int)Math.Ceiling(tour.Price);
            }

            // Check if user has sufficient Adventure Coins for all items
            if (!_walletService.HasSufficientFunds(userId, totalRequiredCoins))
                throw new InvalidOperationException($"Insufficient Adventure Coins. Required: {totalRequiredCoins} AC for all items.");

            // Deduct coins from wallet
            _walletService.DeductCoins(userId, totalRequiredCoins);

            // Record purchases
            cart.PurchaseAllItems(tourPrices);
            _cartRepository.Update(cart);
            _purchaseNotificationService.NotifyToursPurchased(userId, purchasedTourIds);
        }

        private decimal CalculateTotalPrice(Domain.ShoppingCart cart)
        {
            if (cart.Items == null || !cart.Items.Any())
                return 0;

            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var tour = _tourPriceProvider.GetById(item.TourId);
                if (tour != null)
                {
                    total += tour.Price;
                }
            }

            return total;
        }
    }
}
