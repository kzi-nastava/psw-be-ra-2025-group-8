using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
        }

        public CouponDto Create(long authorId, CreateCouponDto dto)
        {
            var code = GenerateCouponCode();
            
            while (_couponRepository.GetByCode(code) != null)
            {
                code = GenerateCouponCode();
            }

            var coupon = new Coupon(code, dto.DiscountPercentage, authorId, dto.ExpiryDate, dto.TourId);
            _couponRepository.Add(coupon);

            return _mapper.Map<CouponDto>(coupon);
        }

        public CouponDto GetById(long id)
        {
            var coupon = _couponRepository.GetById(id);
            if (coupon == null)
                throw new NotFoundException($"Coupon with ID {id} not found.");

            return _mapper.Map<CouponDto>(coupon);
        }

        public CouponDto GetByCode(string code)
        {
            var coupon = _couponRepository.GetByCode(code);
            if (coupon == null)
                throw new NotFoundException($"Coupon with code {code} not found.");

            return _mapper.Map<CouponDto>(coupon);
        }

        public List<CouponDto> GetByAuthorId(long authorId)
        {
            var coupons = _couponRepository.GetByAuthorId(authorId);
            return _mapper.Map<List<CouponDto>>(coupons);
        }

        public CouponDto Update(long id, long authorId, UpdateCouponDto dto)
        {
            var coupon = _couponRepository.GetById(id);
            if (coupon == null)
                throw new NotFoundException($"Coupon with ID {id} not found.");

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only update your own coupons.");

            coupon.Update(dto.DiscountPercentage, dto.ExpiryDate, dto.TourId);
            _couponRepository.Update(coupon);

            return _mapper.Map<CouponDto>(coupon);
        }

        public void Delete(long id, long authorId)
        {
            var coupon = _couponRepository.GetById(id);
            if (coupon == null)
                throw new NotFoundException($"Coupon with ID {id} not found.");

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only delete your own coupons.");

            _couponRepository.Delete(id);
        }

        private static string GenerateCouponCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
    }
}
