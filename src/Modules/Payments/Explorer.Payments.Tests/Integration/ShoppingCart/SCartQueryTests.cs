using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Payments.API.Public;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using Explorer.Payments.Infrastructure.Database;

namespace Explorer.Payments.Tests.Integration.ShoppingCart
{
    [Collection("Sequential")]
    public class SCartQueryTests : BasePaymentsIntegrationTest
    {
        public SCartQueryTests(PaymentsTestFactory factory) : base(factory) { }

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

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

        [Fact]
        public void GetCart_Returns_PurchasedItems()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu, dodaj stavke i kupi ih
            controller.NewCart(-12);
            controller.AddItem(-12, -511);
            controller.AddItem(-12, -522);
            controller.PurchaseItem(-12, -511);

            var actionResult = controller.GetCart(-12);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.PurchasedItems.ShouldNotBeNull();
            cartDto.PurchasedItems.Count.ShouldBe(1);
            cartDto.PurchasedItems.Any(p => p.TourId == -511).ShouldBeTrue();
            cartDto.Items.Count.ShouldBe(1);
        }

        [Fact]
        public void GetCart_Shows_Separate_Items_And_PurchasedItems()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -13);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu sa stavkama i kupljenim stavkama
            controller.NewCart(-13);
            controller.AddItem(-13, -511);
            controller.AddItem(-13, -522);
            controller.AddItem(-13, -533);
            controller.PurchaseItem(-13, -511);
            controller.PurchaseItem(-13, -533);

            var actionResult = controller.GetCart(-13);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            
            // Proveri da postoji 1 stavka u korpi i 2 kupljene stavke
            cartDto.Items.Count.ShouldBe(1);
            cartDto.Items.Any(i => i.TourId == -522).ShouldBeTrue();
            
            cartDto.PurchasedItems.Count.ShouldBe(2);
            cartDto.PurchasedItems.Any(p => p.TourId == -511).ShouldBeTrue();
            cartDto.PurchasedItems.Any(p => p.TourId == -533).ShouldBeTrue();
        }

        [Fact]
        public void GetCart_PurchasedItems_Contain_Price_And_Date()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu i kupi stavku
            controller.NewCart(-12);
            controller.AddItem(-12, -522); // Cena 100
            controller.PurchaseItem(-12, -522);

            var actionResult = controller.GetCart(-12);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.PurchasedItems.Count.ShouldBe(1);

            var purchasedItem = cartDto.PurchasedItems.First();
            purchasedItem.TourId.ShouldBe(-522);
            purchasedItem.Price.ShouldBe(100);
            purchasedItem.PurchaseDate.ShouldNotBe(default(DateTime));
        }

        [Fact]
        public void GetCart_After_PurchaseAll_Shows_All_In_PurchasedItems()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -13);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu, dodaj stavke i kupi sve
            controller.NewCart(-13);
            controller.AddItem(-13, -511);
            controller.AddItem(-13, -522);
            controller.AddItem(-13, -533);
            controller.PurchaseAllItems(-13);

            var actionResult = controller.GetCart(-13);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.Items.Count.ShouldBe(0);
            cartDto.PurchasedItems.Count.ShouldBe(3);
            cartDto.TotalPrice.ShouldBe(0); // TotalPrice samo za Items, ne PurchasedItems
        }

        [Fact]
        public void GetCart_Empty_PurchasedItems_By_Default()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Nova korpa nema kupljenih stavki
            controller.NewCart(-12);

            var actionResult = controller.GetCart(-12);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var cartDto = result.Value as ShoppingCartDto;
            cartDto.ShouldNotBeNull();
            cartDto.PurchasedItems.ShouldNotBeNull();
            cartDto.PurchasedItems.Count.ShouldBe(0);
        }

        [Fact]
        public void GetPurchasedItems_Returns_Only_Purchased_Tours()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu, dodaj stavke i kupi neke
            controller.NewCart(-12);
            controller.AddItem(-12, -511);
            controller.AddItem(-12, -522);
            controller.AddItem(-12, -533);
            controller.PurchaseItem(-12, -511);
            controller.PurchaseItem(-12, -522);

            var actionResult = controller.GetPurchasedItems(-12);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var purchasedItems = result.Value as List<PurchasedItemDto>;
            purchasedItems.ShouldNotBeNull();
            purchasedItems.Count.ShouldBe(2);
            purchasedItems.Any(p => p.TourId == -511).ShouldBeTrue();
            purchasedItems.Any(p => p.TourId == -522).ShouldBeTrue();
            purchasedItems.Any(p => p.TourId == -533).ShouldBeFalse();
        }

        [Fact]
        public void GetPurchasedItems_Returns_Empty_List_When_No_Purchases()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -13);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu bez kupovina
            controller.NewCart(-13);
            controller.AddItem(-13, -511);

            var actionResult = controller.GetPurchasedItems(-13);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var purchasedItems = result.Value as List<PurchasedItemDto>;
            purchasedItems.ShouldNotBeNull();
            purchasedItems.Count.ShouldBe(0);
        }

        [Fact]
        public void GetPurchasedItems_Returns_NotFound_For_Nonexistent_User()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var actionResult = controller.GetPurchasedItems(9999);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(404);
        }

        [Fact]
        public void GetPurchasedItems_Contains_All_Required_Fields()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu i kupi stavku
            controller.NewCart(-12);
            controller.AddItem(-12, -522); // Cena 100
            controller.PurchaseItem(-12, -522);

            var actionResult = controller.GetPurchasedItems(-12);
            var result = actionResult.Result as ObjectResult;

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var purchasedItems = result.Value as List<PurchasedItemDto>;
            purchasedItems.ShouldNotBeNull();
            purchasedItems.Count.ShouldBe(1);

            var item = purchasedItems.First();
            item.Id.ShouldBeGreaterThan(0);
            item.TourId.ShouldBe(-522);
            item.Price.ShouldBe(100);
            item.PurchaseDate.ShouldNotBe(default(DateTime));
        }
    }
}