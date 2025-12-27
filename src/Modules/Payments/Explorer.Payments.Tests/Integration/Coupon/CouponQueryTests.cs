using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Coupon
{
    [Collection("Sequential")]
    public class CouponQueryTests : BasePaymentsIntegrationTest
    {
        public CouponQueryTests(PaymentsTestFactory factory) : base(factory) { }

        private static CouponController CreateController(IServiceScope scope)
        {
            return new CouponController(
                scope.ServiceProvider.GetRequiredService<ICouponService>()
            );
        }

        [Fact]
        public void Gets_coupon_by_id_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Create a coupon first
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                TourId = -511
            };
            var createActionResult = controller.Create(-11, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;

            // Act
            var getActionResult = controller.GetById(created.Id);
            var result = ((ObjectResult)getActionResult.Result)?.Value as CouponDto;

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Code.ShouldBe(created.Code);
            result.DiscountPercentage.ShouldBe(20);
            result.AuthorId.ShouldBe(-11);
        }

        [Fact]
        public void GetById_fails_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.GetById(99999);

            // Assert
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Gets_coupon_by_code_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Create a coupon first
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 25,
                TourId = -522
            };
            var createActionResult = controller.Create(-12, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;

            // Act
            var getActionResult = controller.GetByCode(created.Code);
            var result = ((ObjectResult)getActionResult.Result)?.Value as CouponDto;

            // Assert
            result.ShouldNotBeNull();
            result.Code.ShouldBe(created.Code);
            result.DiscountPercentage.ShouldBe(25);
            result.TourId.ShouldBe(-522);
        }

        [Fact]
        public void GetByCode_fails_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.GetByCode("INVALID1");

            // Assert
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Gets_all_coupons_by_author_id()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Create multiple coupons for author -11
            var dto1 = new CreateCouponDto { DiscountPercentage = 10, TourId = -511 };
            var dto2 = new CreateCouponDto { DiscountPercentage = 20, TourId = -522 };
            var dto3 = new CreateCouponDto { DiscountPercentage = 30, TourId = null };

            controller.Create(-11, dto1);
            controller.Create(-11, dto2);
            controller.Create(-11, dto3);

            // Act
            var getActionResult = controller.GetByAuthorId(-11);
            var result = ((ObjectResult)getActionResult.Result)?.Value as List<CouponDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThanOrEqualTo(3);
            result.All(c => c.AuthorId == -11).ShouldBeTrue();
        }

        [Fact]
        public void GetByAuthorId_returns_empty_list_for_author_without_coupons()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act - Get coupons for author who hasn't created any
            var getActionResult = controller.GetByAuthorId(-99);
            var result = ((ObjectResult)getActionResult.Result)?.Value as List<CouponDto>;

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void Retrieves_coupons_with_different_tour_configurations()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Create coupons with different configurations
            var specificTour = new CreateCouponDto { DiscountPercentage = 15, TourId = -511 };
            var allTours = new CreateCouponDto { DiscountPercentage = 10, TourId = null };

            controller.Create(-11, specificTour);
            controller.Create(-11, allTours);

            // Act
            var getActionResult = controller.GetByAuthorId(-11);
            var result = ((ObjectResult)getActionResult.Result)?.Value as List<CouponDto>;

            // Assert
            result.ShouldNotBeNull();
            var specificCoupon = result.FirstOrDefault(c => c.TourId == -511);
            var allToursCoupon = result.FirstOrDefault(c => c.TourId == null);

            specificCoupon.ShouldNotBeNull();
            allToursCoupon.ShouldNotBeNull();
        }

        [Fact]
        public void Retrieves_coupons_with_and_without_expiry_dates()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Create coupons with different expiry configurations
            var withExpiry = new CreateCouponDto 
            { 
                DiscountPercentage = 20, 
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                TourId = -511 
            };
            var withoutExpiry = new CreateCouponDto 
            { 
                DiscountPercentage = 15, 
                ExpiryDate = null,
                TourId = -522 
            };

            controller.Create(-12, withExpiry);
            controller.Create(-12, withoutExpiry);

            // Act
            var getActionResult = controller.GetByAuthorId(-12);
            var result = ((ObjectResult)getActionResult.Result)?.Value as List<CouponDto>;

            // Assert
            result.ShouldNotBeNull();
            var expiringCoupon = result.FirstOrDefault(c => c.ExpiryDate.HasValue);
            var permanentCoupon = result.FirstOrDefault(c => !c.ExpiryDate.HasValue);

            expiringCoupon.ShouldNotBeNull();
            permanentCoupon.ShouldNotBeNull();
        }

        [Fact]
        public void Coupon_code_is_alphanumeric_and_8_characters()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };

            // Act
            var createActionResult = controller.Create(-11, dto);
            var result = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;

            // Assert
            result.ShouldNotBeNull();
            result.Code.Length.ShouldBe(8);
            result.Code.All(c => char.IsLetterOrDigit(c)).ShouldBeTrue();
        }

        [Fact]
        public void Multiple_authors_can_have_coupons_independently()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new CreateCouponDto { DiscountPercentage = 20, TourId = -511 };

            // Act - Create coupons for different authors
            controller.Create(-11, dto);
            controller.Create(-12, dto);
            controller.Create(-13, dto);

            var get11ActionResult = controller.GetByAuthorId(-11);
            var author11Coupons = ((ObjectResult)get11ActionResult.Result)?.Value as List<CouponDto>;
            var get12ActionResult = controller.GetByAuthorId(-12);
            var author12Coupons = ((ObjectResult)get12ActionResult.Result)?.Value as List<CouponDto>;
            var get13ActionResult = controller.GetByAuthorId(-13);
            var author13Coupons = ((ObjectResult)get13ActionResult.Result)?.Value as List<CouponDto>;

            // Assert - Each author should have their own coupons
            author11Coupons.ShouldNotBeNull();
            author12Coupons.ShouldNotBeNull();
            author13Coupons.ShouldNotBeNull();

            author11Coupons.All(c => c.AuthorId == -11).ShouldBeTrue();
            author12Coupons.All(c => c.AuthorId == -12).ShouldBeTrue();
            author13Coupons.All(c => c.AuthorId == -13).ShouldBeTrue();
        }
    }
}
