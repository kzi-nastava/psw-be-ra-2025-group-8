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

            // Korisnik -21 (turista1) već ima korpu (-500) u test podacima
            var actionResult = controller.GetCart(-21);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.UserId.ShouldBe(-21);
            cartDto.Items.ShouldNotBeNull();
        }

        [Fact]
        public void GetCart_Returns_NotFound_For_Unknown_User()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Korisnik 9999 ne postoji
            var actionResult = controller.GetCart(9999);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void GetCart_Returns_Cart_With_Items()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Korisnik -22 (turista2) ima korpu (-202) sa jednom stavkom (tura -511)
            var actionResult = controller.GetCart(-22);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.UserId.ShouldBe(-22);
            cartDto.Items.ShouldNotBeNull();
            cartDto.Items.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void GetCart_Calculates_Total_Price()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Korisnik -22 (turista2) ima korpu sa turom -511 (cena 50)
            var actionResult = controller.GetCart(-22);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.TotalPrice.ShouldBeGreaterThan(0);
            cartDto.TotalPrice.ShouldBe(50); // Tura -511 ima cenu 50
        }

        [Fact]
        public void GetCart_Returns_Empty_Cart()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Kreiraj novu praznu korpu za turista3 (-23)
            controller.NewCart(-23);
            
            var actionResult = controller.GetCart(-23);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.UserId.ShouldBe(-23);
            cartDto.Items.Count.ShouldBe(0);
            cartDto.TotalPrice.ShouldBe(0);
        }

        [Fact]
        public void GetCart_After_Adding_Items_Shows_Correct_TotalPrice()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Kreiraj novu korpu i dodaj dve ture (cene 100 i 70) za autor2 (-12)
            controller.NewCart(-12);
            controller.AddItem(-12, -522); // Cena 100
            controller.AddItem(-12, -533); // Cena 70

            var actionResult = controller.GetCart(-12);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.TotalPrice.ShouldBe(170); // 100 + 70
        }
    }
}