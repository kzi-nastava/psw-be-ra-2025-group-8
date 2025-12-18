using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.API.Public.ShoppingCart;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Fact]
        public void AddItem_To_Existing_Cart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Korisnik -21 (turista1) već ima praznu korpu (-500) u test podacima
            var result = (ObjectResult)controller.AddItem(-21, -522);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da je stavka dodata u bazu
            dbContext.ChangeTracker.Clear(); // Osveži kontekst
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == -21);
            cart.ShouldNotBeNull();
            cart.Items.Any(i => i.TourId == -522).ShouldBeTrue();
        }

        [Fact]
        public void AddItem_With_NonExisting_Tour_Returns_NotFound()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Pokušaj dodavanja nepostojeće ture
            var result = (ObjectResult)controller.AddItem(-21, 9999);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void RemoveItem_From_Cart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Korisnik -22 (turista2) ima korpu (-202) sa jednom stavkom (tura -511)
            var result = (ObjectResult)controller.RemoveItem(-22, -511);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da je stavka uklonjena
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == -22);
            cart.ShouldNotBeNull();
            cart.Items.Any(i => i.TourId == -511).ShouldBeFalse();
        }

        [Fact]
        public void ClearCart_Removes_All_Items()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Kreiraj novu korpu i dodaj stavke za turista3 (-23)
            controller.NewCart(-23);
            controller.AddItem(-23, -511);
            controller.AddItem(-23, -533);

            dbContext.ChangeTracker.Clear();

            var result = (ObjectResult)controller.ClearCart(-23);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da su sve stavke uklonjene
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == -23);
            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(0);
        }

        [Fact]
        public void CreateCart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Kreiraj novu korpu za turista3 (-23)
            var result = (ObjectResult)controller.NewCart(-23);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da je korpa kreirana
            dbContext.ChangeTracker.Clear();
            var newCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -23);
            newCart.ShouldNotBeNull();
            newCart.UserId.ShouldBe(-23);
        }

        [Fact]
        public void DeleteCart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Kreiraj korpu za autor2 (-12) pa je obriši
            controller.NewCart(-12);
            dbContext.ChangeTracker.Clear();

            var result = (ObjectResult)controller.DeleteCart(-12);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da je korpa obrisana
            dbContext.ChangeTracker.Clear();
            var deletedCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            deletedCart.ShouldBeNull();
        }

        [Fact]
        public void AddItem_Multiple_Tours_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            // Kreiraj novu korpu i dodaj više tura za autor3 (-13)
            controller.NewCart(-13);
            
            var result1 = (ObjectResult)controller.AddItem(-13, -511);
            result1.StatusCode.ShouldBe(200);

            var result2 = (ObjectResult)controller.AddItem(-13, -533);
            result2.StatusCode.ShouldBe(200);

            // Verifikuj da su obe ture dodate
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .FirstOrDefault(c => c.UserId == -13);
            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(2);
            cart.Items.Any(i => i.TourId == -511).ShouldBeTrue();
            cart.Items.Any(i => i.TourId == -533).ShouldBeTrue();
        }

        [Fact]
        public void ClearCart_For_Nonexistent_Cart_Returns_NotFound()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Pokušaj da obrišeš nepostojeću korpu
            var result = (ObjectResult)controller.ClearCart(9999);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void CreateCart_For_Existing_User_Returns_BadRequest()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Korisnik -21 (turista1) već ima korpu u test podacima
            var result = (ObjectResult)controller.NewCart(-21);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void DeleteCart_For_Nonexistent_Cart_Returns_NotFound()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            // Pokušaj da obrišeš nepostojeću korpu
            var result = (ObjectResult)controller.DeleteCart(9999);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }
    }
}