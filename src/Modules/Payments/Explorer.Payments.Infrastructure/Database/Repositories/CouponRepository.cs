using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Payments.Infrastructure.Database;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly PaymentsContext _context;

        public CouponRepository(PaymentsContext context)
        {
            _context = context;
        }

        public Coupon? GetById(long id)
        {
            return _context.Coupons.FirstOrDefault(c => c.Id == id);
        }

        public Coupon? GetByCode(string code)
        {
            return _context.Coupons.FirstOrDefault(c => c.Code == code);
        }

        public List<Coupon> GetByAuthorId(long authorId)
        {
            return _context.Coupons.Where(c => c.AuthorId == authorId).ToList();
        }

        public void Add(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
            _context.SaveChanges();
        }

        public void Update(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var coupon = GetById(id);
            if (coupon != null)
            {
                _context.Coupons.Remove(coupon);
                _context.SaveChanges();
            }
        }
    }
}
