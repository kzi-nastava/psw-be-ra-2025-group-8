using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICouponRepository
    {
        Coupon? GetById(long id);
        Coupon? GetByCode(string code);
        List<Coupon> GetByAuthorId(long authorId);
        void Add(Coupon coupon);
        void Update(Coupon coupon);
        void Delete(long id);
    }
}
