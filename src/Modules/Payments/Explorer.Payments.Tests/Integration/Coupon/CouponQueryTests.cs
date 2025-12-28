using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Explorer.Payments.Tests.Integration.Coupon
{
    [Collection("Sequential")]
    public class CouponQueryTests : BasePaymentsIntegrationTest
    {
        public CouponQueryTests(PaymentsTestFactory factory) : base(factory) { }

        private static CouponController CreateController(IServiceScope scope, long authorId)
        {
            var controller = new CouponController(
                scope.ServiceProvider.GetRequiredService<ICouponService>()
            );

            // Mock the authentication context
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("id", authorId.ToString()),
                        new Claim("personId", authorId.ToString()),
                        new Claim(ClaimTypes.Role, "author")
                    }))
                }
            };

            return controller;
        }

        [Fact]
        public void Gets_coupon_by_id_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope, -11);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Create a coupon first
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                TourId = -511
            };
            var createActionResult = controller.Create(createDto);
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
            var controller = CreateController(scope, -11);

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
            var controller = CreateController(scope, -12);

            // Create a coupon first
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 25,
                TourId = -522
            };
            var createActionResult = controller.Create(createDto);
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
            var controller = CreateController(scope, -11);

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
            var controller = CreateController(scope, -11);

            // Create multiple coupons for author -11
            var dto1 = new CreateCouponDto { DiscountPercentage = 10, TourId = -511 };
            var dto2 = new CreateCouponDto { DiscountPercentage = 20, TourId = -522 };
            var dto3 = new CreateCouponDto { DiscountPercentage = 30, TourId = null };

            controller.Create(dto1);
            controller.Create(dto2);
            controller.Create(dto3);

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
            var controller = CreateController(scope, -99);

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
            var controller = CreateController(scope, -11);

            // Create coupons with different configurations
            var specificTour = new CreateCouponDto { DiscountPercentage = 15, TourId = -511 };
            var allTours = new CreateCouponDto { DiscountPercentage = 10, TourId = null };

            controller.Create(specificTour);
            controller.Create(allTours);

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
            var controller = CreateController(scope, -12);

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

            controller.Create(withExpiry);
            controller.Create(withoutExpiry);

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
            var controller = CreateController(scope, -11);

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };

            // Act
            var createActionResult = controller.Create(dto);
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
            var controller11 = CreateController(scope, -11);
            var controller12 = CreateController(scope, -12);
            var controller13 = CreateController(scope, -13);

            var dto = new CreateCouponDto { DiscountPercentage = 20, TourId = -511 };

            // Act - Create coupons for different authors
            controller11.Create(dto);
            controller12.Create(dto);
            controller13.Create(dto);

            var get11ActionResult = controller11.GetByAuthorId(-11);
            var author11Coupons = ((ObjectResult)get11ActionResult.Result)?.Value as List<CouponDto>;
            var get12ActionResult = controller12.GetByAuthorId(-12);
            var author12Coupons = ((ObjectResult)get12ActionResult.Result)?.Value as List<CouponDto>;
            var get13ActionResult = controller13.GetByAuthorId(-13);
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
