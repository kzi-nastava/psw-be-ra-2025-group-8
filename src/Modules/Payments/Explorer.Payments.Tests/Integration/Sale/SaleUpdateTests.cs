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
    public class SaleUpdateTests : BasePaymentsIntegrationTest
    {
        public SaleUpdateTests(PaymentsTestFactory factory) : base(factory) { }

        private static SaleController CreateController(IServiceScope scope)
        {
            return new SaleController(scope.ServiceProvider.GetRequiredService<ISaleService>());
        }

        [Fact]
        public void Updates_sale_successfully()
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

            var updateDto = new UpdateSaleDto
            {
                StartDate = DateTime.UtcNow.Date.AddDays(2),
                EndDate = DateTime.UtcNow.Date.AddDays(6),
                DiscountPercentage = 30,
                TourIds = new List<long> { -511, -522 }
            };

            var updated = ((ObjectResult)controller.Update(created.Id, updateDto, authorId: -11).Result)?.Value as SaleDto;
            updated.ShouldNotBeNull();
            updated.DiscountPercentage.ShouldBe(30);
            updated.TourIds.Count.ShouldBe(2);

            dbContext.ChangeTracker.Clear();
            var stored = dbContext.Sales.FirstOrDefault(s => s.Id == created.Id);
            stored.ShouldNotBeNull();
            dbContext.SaleTours.Count(st => st.SaleId == created.Id).ShouldBe(2);
        }
    }
}
