using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.ShoppingCart;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;
namespace Explorer.Tours.Core.UseCases.ShoppingCart
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public ShoppingCartService(IShoppingCartRepository cartRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
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

            var tour = _tourRepository.Get(tourId);
            if (tour == null) throw new NotFoundException("Tour not found.");

            cart.PurchaseItem(tourId, tour.Price);
            _cartRepository.Update(cart);
        }

        public void PurchaseAllItems(long userId)
        {
            var cart = _cartRepository.GetByUserId(userId);
            if (cart == null) throw new NotFoundException("Cart not found for this user.");

            if (!cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var tourPrices = new Dictionary<long, decimal>();
            foreach (var item in cart.Items)
            {
                var tour = _tourRepository.Get(item.TourId);
                if (tour == null)
                    throw new NotFoundException($"Tour with ID {item.TourId} not found.");
                
                tourPrices[item.TourId] = tour.Price;
            }

            cart.PurchaseAllItems(tourPrices);
            _cartRepository.Update(cart);
        }

        private decimal CalculateTotalPrice(Domain.ShoppingCart cart)
        {
            if (cart.Items == null || !cart.Items.Any())
                return 0;

            decimal total = 0;

            foreach (var item in cart.Items)
            {
                var tour = _tourRepository.Get(item.TourId);
                if (tour != null)
                {
                    total += tour.Price;
                }
            }

            return total;
        }


    }
}
