using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.Sale
{
    [Collection("Sequential")]
    public class SaleCommandTests : BasePaymentsIntegrationTest
    {
        public SaleCommandTests(PaymentsTestFactory factory) : base(factory) { }

        private static SaleController CreateController(IServiceScope scope)
        {
            return new SaleController(scope.ServiceProvider.GetRequiredService<ISaleService>());
        }

        [Fact]
        public void Creates_sale_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var dto = new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(5),
                DiscountPercentage = 20,
                TourIds = new List<long> { -511, -522 }
            };

            var actionResult = controller.Create(dto, authorId: -11);
            var result = ((ObjectResult)actionResult.Result)?.Value as SaleDto;

            result.ShouldNotBeNull();
            result.AuthorId.ShouldBe(-11);
            result.DiscountPercentage.ShouldBe(20);
            result.TourIds.Count.ShouldBe(2);
            result.TourIds.ShouldContain(-511);
            result.TourIds.ShouldContain(-522);

            dbContext.ChangeTracker.Clear();
            var stored = dbContext.Sales.FirstOrDefault(s => s.Id == result.Id);
            stored.ShouldNotBeNull();
            dbContext.SaleTours.Count(st => st.SaleId == stored.Id).ShouldBe(2);
        }

        [Fact]
        public void Create_fails_when_tour_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(3),
                DiscountPercentage = 10,
                TourIds = new List<long> { 99999 }
            };

            var actionResult = controller.Create(dto, authorId: -11);
            actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Create_fails_when_tour_does_not_belong_to_author()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var dto = new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(3),
                DiscountPercentage = 10,
                TourIds = new List<long> { -544 }
            };

            var actionResult = controller.Create(dto, authorId: -11);

            // controller uses Forbid for UnauthorizedAccessException
            actionResult.Result.ShouldBeOfType<ForbidResult>();
        }

        [Fact]
        public void Update_fails_when_not_owner()
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

            var updateDto = new UpdateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(2),
                EndDate = DateTime.UtcNow.Date.AddDays(6),
                DiscountPercentage = 30,
                TourIds = new List<long> { -511 }
            };

            var actionResult = controller.Update(created.Id, updateDto, authorId: -12);
            actionResult.Result.ShouldBeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public void Deletes_sale_successfully()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var createDto = new CreateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                EndDate = DateTime.UtcNow.Date.AddDays(5),
                DiscountPercentage = 20,
                TourIds = new List<long> { -511 }
            };
            var created = ((ObjectResult)controller.Create(createDto, authorId: -11).Result)?.Value as SaleDto;
            created.ShouldNotBeNull();

            var deleteResult = controller.Delete(created.Id, authorId: -11);
            deleteResult.ShouldBeOfType<OkObjectResult>();

            dbContext.ChangeTracker.Clear();
            dbContext.Sales.FirstOrDefault(s => s.Id == created.Id).ShouldBeNull();
        }

        [Fact]
        public void Delete_fails_not_found()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var actionResult = controller.Delete(99999, authorId: -11);
            actionResult.ShouldBeOfType<NotFoundObjectResult>();
        }
    }
}
