using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Sale
{
    [Collection("Sequential")]
    public class SaleQueryTests : BasePaymentsIntegrationTest
    {
        public SaleQueryTests(PaymentsTestFactory factory) : base(factory) { }

        private static SaleController CreateController(IServiceScope scope)
        {
            return new SaleController(scope.ServiceProvider.GetRequiredService<ISaleService>());
        }

        [Fact]
        public void Gets_sale_by_id_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var createDto = new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(5),
                DiscountPercentage = 20,
                TourIds = new List<long> { -511 }
            };

            var created = ((ObjectResult)controller.Create(createDto, authorId: -11).Result)?.Value as SaleDto;
            created.ShouldNotBeNull();

            var getResult = controller.Get(created.Id);
            var dto = ((ObjectResult)getResult.Result)?.Value as SaleDto;

            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(created.Id);
            dto.AuthorId.ShouldBe(-11);
            dto.TourIds.Single().ShouldBe(-511);
        }

        [Fact]
        public void Get_fails_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = controller.Get(99999);
            result.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Gets_sales_by_author()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            controller.Create(new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(5),
                DiscountPercentage = 10,
                TourIds = new List<long> { -511 }
            }, authorId: -11);

            controller.Create(new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(2),
                EndDate = DateTime.UtcNow.Date.AddDays(6),
                DiscountPercentage = 20,
                TourIds = new List<long> { -522 }
            }, authorId: -11);

            var result = controller.GetByAuthor(authorId: -11);
            var list = ((ObjectResult)result.Result)?.Value as List<SaleDto>;

            list.ShouldNotBeNull();
            list.Count.ShouldBeGreaterThanOrEqualTo(2);
            list.All(s => s.AuthorId == -11).ShouldBeTrue();
        }

        [Fact]
        public void Gets_active_sales()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Active sale: start in past, end in future is invalid by domain (start cannot be in past).
            // Create sale that starts today and ends tomorrow - may or may not be active depending on current time.
            // To ensure active, use start today (UtcNow.Date) and end today + 1 day (still >= now).
            controller.Create(new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(1),
                DiscountPercentage = 15,
                TourIds = new List<long> { -511 }
            }, authorId: -11);

            var result = controller.GetActiveSales();
            var list = ((ObjectResult)result.Result)?.Value as List<SaleDto>;

            list.ShouldNotBeNull();
            list.Count.ShouldBeGreaterThanOrEqualTo(1);
            list.Any(s => s.DiscountPercentage == 15).ShouldBeTrue();
        }

        [Fact]
        public void Gets_paged_sales()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            for (var i = 0; i < 3; i++)
            {
                controller.Create(new CreateSaleDto
                {
                    StartDate = DateTime.UtcNow.Date.AddDays(1 + i),
                    EndDate = DateTime.UtcNow.Date.AddDays(2 + i),
                    DiscountPercentage = 10 + i,
                    TourIds = new List<long> { -511 }
                }, authorId: -11);
            }

            var result = controller.GetPaged(page: 1, pageSize: 2);
            var paged = ((ObjectResult)result.Result)?.Value as PagedResult<SaleDto>;

            paged.ShouldNotBeNull();
            paged.Results.Count.ShouldBeLessThanOrEqualTo(2);
            paged.TotalCount.ShouldBeGreaterThanOrEqualTo(3);
        }
    }
}
