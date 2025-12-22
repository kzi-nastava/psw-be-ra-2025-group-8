using Explorer.API.Controllers.Tourist;
using Explorer.Payments.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Payments.Tests.Integration.ShoppingCart
{
    [Collection("Sequential")]
    public class SCartCommandTests : BasePaymentsIntegrationTest
    {
        public SCartCommandTests(PaymentsTestFactory factory) : base(factory) { }

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

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
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts.FirstOrDefault(c => c.UserId == -13);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

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

        [Fact]
        public void PurchaseItem_From_Cart_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Prvo obriši postojeću korpu ako postoji
            var existingCart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -12);
            if (existingCart != null)
            {
                dbContext.ShoppingCarts.Remove(existingCart);
                dbContext.SaveChanges();
            }
            dbContext.ChangeTracker.Clear();

            // Kreiraj korpu i dodaj stavke za autor2 (-12)
            controller.NewCart(-12);
            controller.AddItem(-12, -511);
            controller.AddItem(-12, -522);

            dbContext.ChangeTracker.Clear();

            // Kupi jednu stavku
            var result = (ObjectResult)controller.PurchaseItem(-12, -511);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da je stavka premestena iz Items u PurchasedItems
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -12);
            
            cart.ShouldNotBeNull();
            cart.Items.Any(i => i.TourId == -511).ShouldBeFalse();
            cart.PurchasedItems.Any(p => p.TourId == -511).ShouldBeTrue();
            cart.Items.Count.ShouldBe(1);
            cart.PurchasedItems.Count.ShouldBe(1);
        }

        [Fact]
        public void PurchaseItem_Nonexistent_Tour_Returns_NotFound()
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

            // Kreiraj korpu za autor3 (-13)
            controller.NewCart(-13);
            controller.AddItem(-13, -511);

            // Pokušaj da kupiš stavku koja nije u korpi
            var result = (ObjectResult)controller.PurchaseItem(-13, -522);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void PurchaseItem_Saves_Correct_Price()
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

            // Kreiraj korpu i dodaj turu sa poznatom cenom
            controller.NewCart(-13);
            controller.AddItem(-13, -522); // Tura -522 ima cenu 100

            dbContext.ChangeTracker.Clear();

            // Kupi stavku
            controller.PurchaseItem(-13, -522);

            // Verifikuj da je cena sačuvana
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -13);

            cart.ShouldNotBeNull();
            var purchasedItem = cart.PurchasedItems.FirstOrDefault(p => p.TourId == -522);
            purchasedItem.ShouldNotBeNull();
            purchasedItem.Price.ShouldBe(100);
        }

        [Fact]
        public void PurchaseAllItems_Succeeds()
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

            // Kreiraj korpu i dodaj više stavki
            controller.NewCart(-12);
            controller.AddItem(-12, -511); // Cena 50
            controller.AddItem(-12, -522); // Cena 100
            controller.AddItem(-12, -533); // Cena 70

            dbContext.ChangeTracker.Clear();

            // Kupi sve stavke
            var result = (ObjectResult)controller.PurchaseAllItems(-12);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            // Verifikuj da su sve stavke premestene
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -12);

            cart.ShouldNotBeNull();
            cart.Items.Count.ShouldBe(0);
            cart.PurchasedItems.Count.ShouldBe(3);
            cart.PurchasedItems.Any(p => p.TourId == -511).ShouldBeTrue();
            cart.PurchasedItems.Any(p => p.TourId == -522).ShouldBeTrue();
            cart.PurchasedItems.Any(p => p.TourId == -533).ShouldBeTrue();
        }

        [Fact]
        public void PurchaseAllItems_Empty_Cart_Returns_BadRequest()
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

            // Kreiraj praznu korpu
            controller.NewCart(-13);

            // Pokušaj da kupiš sve iz prazne korpe
            var result = (ObjectResult)controller.PurchaseAllItems(-13);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(400);
        }

        [Fact]
        public void PurchaseAllItems_Calculates_Total_Price_Correctly()
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

            // Kreiraj korpu i dodaj stavke
            controller.NewCart(-13);
            controller.AddItem(-13, -511); // Cena 50
            controller.AddItem(-13, -533); // Cena 70

            dbContext.ChangeTracker.Clear();

            // Kupi sve stavke
            controller.PurchaseAllItems(-13);

            // Verifikuj ukupnu cenu
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -13);

            cart.ShouldNotBeNull();
            var totalPrice = cart.PurchasedItems.Sum(p => p.Price);
            totalPrice.ShouldBe(120); // 50 + 70
        }

        [Fact]
        public void PurchasedItems_Have_PurchaseDate()
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

            var beforePurchase = DateTime.UtcNow.AddSeconds(-1);

            // Kreiraj korpu i kupi stavku
            controller.NewCart(-12);
            controller.AddItem(-12, -511);
            controller.PurchaseItem(-12, -511);

            var afterPurchase = DateTime.UtcNow.AddSeconds(1);

            // Verifikuj da PurchaseDate postoji i da je u odgovarajućem vremenskom okviru
            dbContext.ChangeTracker.Clear();
            var cart = dbContext.ShoppingCarts
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -12);

            cart.ShouldNotBeNull();
            var purchasedItem = cart.PurchasedItems.FirstOrDefault(p => p.TourId == -511);
            purchasedItem.ShouldNotBeNull();
            purchasedItem.PurchaseDate.ShouldBeGreaterThanOrEqualTo(beforePurchase);
            purchasedItem.PurchaseDate.ShouldBeLessThanOrEqualTo(afterPurchase);
        }

        [Fact]
        public void Multiple_Purchases_Accumulate_In_PurchasedItems()
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

            // Kreiraj korpu i dodaj stavke
            controller.NewCart(-13);
            controller.AddItem(-13, -511);
            controller.AddItem(-13, -522);
            controller.AddItem(-13, -533);

            // Kupi stavke jednu po jednu
            controller.PurchaseItem(-13, -511);
            dbContext.ChangeTracker.Clear();
            
            controller.PurchaseItem(-13, -522);
            dbContext.ChangeTracker.Clear();

            // Verifikuj da se kupljene stavke akumuliraju
            var cart = dbContext.ShoppingCarts
                .Include(c => c.Items)
                .Include(c => c.PurchasedItems)
                .FirstOrDefault(c => c.UserId == -13);

            cart.ShouldNotBeNull();
            cart.PurchasedItems.Count.ShouldBe(2);
            cart.Items.Count.ShouldBe(1);
            cart.PurchasedItems.Any(p => p.TourId == -511).ShouldBeTrue();
            cart.PurchasedItems.Any(p => p.TourId == -522).ShouldBeTrue();
        }
    }
}