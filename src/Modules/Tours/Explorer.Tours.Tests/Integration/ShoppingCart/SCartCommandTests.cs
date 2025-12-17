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
    public class SCartCommandTests : BaseToursIntegrationTest
    {
        public SCartCommandTests(ToursTestFactory factory) : base(factory) { }

        private static ShoppingCartController CreateController(IServiceScope scope)
        {
            return new ShoppingCartController(
                scope.ServiceProvider.GetRequiredService<IShoppingCartService>(),
                scope.ServiceProvider.GetRequiredService<ITourService>()
            );
        }

        // -----------------------------
        // ADD ITEM TO CART
        // -----------------------------
        [Theory]
        [InlineData(-41, -511, 200)] // valid user, valid tour
        [InlineData(-41, 9999, 404)] // valid user, non-existing tour
        public void AddItem_To_Cart(long userId, long tourId, int expectedStatus)
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.NewCart(userId);

            var result = (ObjectResult)controller.AddItem(userId, tourId);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(expectedStatus);

            if (expectedStatus == 200)
            {
                var cart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);
                cart.ShouldNotBeNull();
                cart.Items.Any(i => i.TourId == tourId).ShouldBeTrue();
            }
        }

        // -----------------------------
        // REMOVE ITEM
        // -----------------------------
        [Fact]
        public void RemoveItem_From_Cart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.NewCart(-42);
            controller.AddItem(-42, -511);

            var result = (ObjectResult)controller.RemoveItem(-42, -511);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -42);
            cart.ShouldNotBeNull();
            cart.Items.Any(i => i.TourId == -511).ShouldBeFalse();
        }

        // -----------------------------
        // CLEAR CART
        // -----------------------------
        [Fact]
        public void ClearCart_Removes_All_Items()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Kreiraj korpu i dodaj 2 ture
            controller.NewCart(-41);
            controller.AddItem(-41, -522);
            controller.AddItem(-41, -533);

            var result = (ObjectResult)controller.ClearCart(-41);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -41);
            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(0);
        }

        // -----------------------------
        // CREATE CART
        // -----------------------------
        [Fact]
        public void CreateCart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = (ObjectResult)controller.NewCart(-43);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var newCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -43);
            newCart.ShouldNotBeNull();
        }

        // -----------------------------
        // DELETE CART
        // -----------------------------
        [Fact]
        public void DeleteCart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            controller.NewCart(-44);

            var result = (ObjectResult)controller.DeleteCart(-44);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var deletedCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -44);
            deletedCart.ShouldBeNull();
        }
    }
}
