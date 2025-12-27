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
    public class CouponCommandTests : BasePaymentsIntegrationTest
    {
        public CouponCommandTests(PaymentsTestFactory factory) : base(factory) { }

        private static CouponController CreateController(IServiceScope scope)
        {
            return new CouponController(
                scope.ServiceProvider.GetRequiredService<ICouponService>()
            );
        }

        [Fact]
        public void Creates_coupon_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                TourId = -511
            };

            // Act
            var actionResult = controller.Create(-11, dto);
            
            // Debug: Check what type of result we got
            actionResult.Result.ShouldNotBeNull();
            actionResult.Result.ShouldBeOfType<OkObjectResult>();
            
            var result = ((OkObjectResult)actionResult.Result)?.Value as CouponDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Code.ShouldNotBeNullOrEmpty();
            result.Code.Length.ShouldBe(8);
            result.DiscountPercentage.ShouldBe(20);
            result.AuthorId.ShouldBe(-11);
            result.TourId.ShouldBe(-511);

            // Assert - Database
            dbContext.ChangeTracker.Clear();
            var storedCoupon = dbContext.Coupons.FirstOrDefault(c => c.Id == result.Id);
            storedCoupon.ShouldNotBeNull();
            storedCoupon.Code.ShouldBe(result.Code);
            storedCoupon.DiscountPercentage.ShouldBe(20);
        }

        [Fact]
        public void Creates_coupon_for_all_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 15,
                ExpiryDate = null,
                TourId = null // Applies to all tours
            };

            // Act
            var actionResult = controller.Create(-11, dto);
            var result = ((ObjectResult)actionResult.Result)?.Value as CouponDto;

            // Assert
            result.ShouldNotBeNull();
            result.TourId.ShouldBeNull();
            result.ExpiryDate.ShouldBeNull();

            // Assert - Database
            dbContext.ChangeTracker.Clear();
            var storedCoupon = dbContext.Coupons.FirstOrDefault(c => c.Id == result.Id);
            storedCoupon.ShouldNotBeNull();
            storedCoupon.TourId.ShouldBeNull();
        }

        [Fact]
        public void Creates_coupon_without_expiry_date()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 10,
                ExpiryDate = null,
                TourId = -522
            };

            // Act
            var actionResult = controller.Create(-12, dto);
            var result = ((ObjectResult)actionResult.Result)?.Value as CouponDto;

            // Assert
            result.ShouldNotBeNull();
            result.ExpiryDate.ShouldBeNull();
        }

        [Fact]
        public void Create_fails_invalid_discount_percentage()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 150, // Invalid: > 100
                TourId = -511
            };

            // Act
            var result = controller.Create(-11, dto);

            // Assert
            result.Result.ShouldBeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void Updates_coupon_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // First create a coupon
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                TourId = -511
            };
            var createActionResult = controller.Create(-11, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;
            dbContext.ChangeTracker.Clear();

            // Update the coupon
            var updateDto = new UpdateCouponDto
            {
                DiscountPercentage = 30,
                ExpiryDate = DateTime.UtcNow.AddDays(60),
                TourId = -522
            };

            // Act
            var updateActionResult = controller.Update(created.Id, -11, updateDto);
            var result = ((ObjectResult)updateActionResult.Result)?.Value as CouponDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.DiscountPercentage.ShouldBe(30);
            result.TourId.ShouldBe(-522);

            // Assert - Database
            dbContext.ChangeTracker.Clear();
            var storedCoupon = dbContext.Coupons.FirstOrDefault(c => c.Id == created.Id);
            storedCoupon.ShouldNotBeNull();
            storedCoupon.DiscountPercentage.ShouldBe(30);
            storedCoupon.TourId.ShouldBe(-522);
        }

        [Fact]
        public void Update_fails_unauthorized()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Create coupon as author -11
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };
            var createActionResult = controller.Create(-11, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;
            dbContext.ChangeTracker.Clear();

            // Try to update as different author -12
            var updateDto = new UpdateCouponDto
            {
                DiscountPercentage = 30
            };

            // Act
            var result = controller.Update(created.Id, -12, updateDto);

            // Assert
            var objectResult = result.Result.ShouldBeOfType<ObjectResult>();
            objectResult.StatusCode.ShouldBe(403);
        }

        [Fact]
        public void Deletes_coupon_successfully()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Create coupon
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };
            var createActionResult = controller.Create(-11, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;
            dbContext.ChangeTracker.Clear();

            // Act
            var result = controller.Delete(created.Id, -11);

            // Assert - Response
            result.ShouldBeOfType<OkObjectResult>();

            // Assert - Database
            dbContext.ChangeTracker.Clear();
            var storedCoupon = dbContext.Coupons.FirstOrDefault(c => c.Id == created.Id);
            storedCoupon.ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_unauthorized()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Create coupon as author -11
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };
            var createActionResult = controller.Create(-11, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;
            dbContext.ChangeTracker.Clear();

            // Try to delete as different author -12
            // Act
            var result = controller.Delete(created.Id, -12);

            // Assert
            var objectResult = result.ShouldBeOfType<ObjectResult>();
            objectResult.StatusCode.ShouldBe(403);
        }

        [Fact]
        public void Delete_fails_not_found()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Act
            var result = controller.Delete(99999, -11);

            // Assert
            result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Generated_coupon_codes_are_unique()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };

            // Act - Create multiple coupons
            var actionResult1 = controller.Create(-11, dto);
            var result1 = ((ObjectResult)actionResult1.Result)?.Value as CouponDto;
            var actionResult2 = controller.Create(-11, dto);
            var result2 = ((ObjectResult)actionResult2.Result)?.Value as CouponDto;
            var actionResult3 = controller.Create(-11, dto);
            var result3 = ((ObjectResult)actionResult3.Result)?.Value as CouponDto;

            // Assert - All codes should be unique
            result1.Code.ShouldNotBe(result2.Code);
            result1.Code.ShouldNotBe(result3.Code);
            result2.Code.ShouldNotBe(result3.Code);

            // Assert - Database uniqueness
            dbContext.ChangeTracker.Clear();
            var codes = dbContext.Coupons
                .Where(c => c.AuthorId == -11)
                .Select(c => c.Code)
                .ToList();
            codes.Distinct().Count().ShouldBe(codes.Count);
        }

        [Fact]
        public void Updates_coupon_to_apply_for_all_tours()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Create coupon for specific tour
            var createDto = new CreateCouponDto
            {
                DiscountPercentage = 20,
                TourId = -511
            };
            var createActionResult = controller.Create(-11, createDto);
            var created = ((ObjectResult)createActionResult.Result)?.Value as CouponDto;
            dbContext.ChangeTracker.Clear();

            // Update to apply for all tours
            var updateDto = new UpdateCouponDto
            {
                DiscountPercentage = 25,
                TourId = null // Now applies to all tours
            };

            // Act
            var updateActionResult = controller.Update(created.Id, -11, updateDto);
            var result = ((ObjectResult)updateActionResult.Result)?.Value as CouponDto;

            // Assert
            result.ShouldNotBeNull();
            result.TourId.ShouldBeNull();

            // Assert - Database
            dbContext.ChangeTracker.Clear();
            var storedCoupon = dbContext.Coupons.FirstOrDefault(c => c.Id == created.Id);
            storedCoupon.ShouldNotBeNull();
            storedCoupon.TourId.ShouldBeNull();
        }
    }
}
