using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly PaymentsContext _context;

        public ShoppingCartRepository(PaymentsContext context)
        {
            _context = context;
        }
        public void CreateCart(ShoppingCart cart)
        {
            _context.ShoppingCarts.Add(cart);
            _context.SaveChanges();
        }
        public ShoppingCart GetById(long id)
        {
            return _context.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.Id == id);
        }

        public ShoppingCart GetByUserId(long userId)
        {
            return _context.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == userId);
        }

        public void Add(ShoppingCart cart)
        {
            _context.ShoppingCarts.Add(cart);
            _context.SaveChanges();
        }

        public void Update(ShoppingCart cart)
        {
            _context.ShoppingCarts.Update(cart);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var existing = _context.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.Id == id);

            if (existing != null)
            {
                _context.ShoppingCarts.Remove(existing);
                _context.SaveChanges();
            }
        }
    }
}
