using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface ICouponService
    {
        CouponDto Create(long authorId, CreateCouponDto dto);
        CouponDto GetById(long id);
        CouponDto GetByCode(string code);
        List<CouponDto> GetByAuthorId(long authorId);
        CouponDto Update(long id, long authorId, UpdateCouponDto dto);
        void Delete(long id, long authorId);
    }
}
