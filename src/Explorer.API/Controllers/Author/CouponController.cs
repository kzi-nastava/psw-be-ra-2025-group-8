using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Author
{
    //[Authorize(Policy = "authorPolicy")]
    [Route("api/author/coupons")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpPost]
        public ActionResult<CouponDto> Create([FromQuery] long authorId, [FromBody] CreateCouponDto dto)
        {
            try
            {
                var coupon = _couponService.Create(authorId, dto);
                return Ok(coupon);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CouponDto> GetById(long id)
        {
            try
            {
                var coupon = _couponService.GetById(id);
                return Ok(coupon);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("code/{code}")]
        public ActionResult<CouponDto> GetByCode(string code)
        {
            try
            {
                var coupon = _couponService.GetByCode(code);
                return Ok(coupon);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("author/{authorId}")]
        public ActionResult<List<CouponDto>> GetByAuthorId(long authorId)
        {
            var coupons = _couponService.GetByAuthorId(authorId);
            return Ok(coupons);
        }

        [HttpPut("{id}")]
        public ActionResult<CouponDto> Update(long id, [FromQuery] long authorId, [FromBody] UpdateCouponDto dto)
        {
            try
            {
                var coupon = _couponService.Update(id, authorId, dto);
                return Ok(coupon);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id, [FromQuery] long authorId)
        {
            try
            {
                _couponService.Delete(id, authorId);
                return Ok("Coupon deleted successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
