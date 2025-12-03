using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.ShoppingCart;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests.Integration.ShoppingCart
{
    [Collection("Sequential")]
    public class SCartQueryTests : BaseToursIntegrationTest
    {
        public SCartQueryTests(ToursTestFactory factory) : base(factory) { }

        private static ShoppingCartController CreateController(IServiceScope scope)
        {
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>()
            );
        }

        [Fact]
        public void GetCart_Returns_Existing_Cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = (ObjectResult)controller.GetCart(-11).Result;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.UserId.ShouldBe(-11);
            cartDto.Items.ShouldNotBeNull();
        }

        [Fact]
        public void GetCart_Returns_NotFound_For_Unknown_User()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var result = (ObjectResult)controller.GetCart(9999).Result;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }
    }
}
