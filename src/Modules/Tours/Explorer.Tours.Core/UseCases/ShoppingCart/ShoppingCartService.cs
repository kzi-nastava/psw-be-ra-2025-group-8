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
        private readonly IMapper _mapper;

        public ShoppingCartService(IShoppingCartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
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

            return _mapper.Map<ShoppingCartDto>(cart);
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
    }
}
