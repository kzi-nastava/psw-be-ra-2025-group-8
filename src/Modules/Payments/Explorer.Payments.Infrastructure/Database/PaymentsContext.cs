using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database;

public class PaymentsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PurchasedItem> PurchasedItems { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.Entity<ShoppingCart>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.UserId).IsRequired();
            
            builder.HasMany(c => c.Items)
                   .WithOne()
                   .HasForeignKey("ShoppingCartId")
                   .OnDelete(DeleteBehavior.Cascade);
            
            // Configure backing field for Items collection
            var itemsNav = builder.Metadata.FindNavigation(nameof(ShoppingCart.Items));
            itemsNav.SetPropertyAccessMode(PropertyAccessMode.Field);
            itemsNav.SetField("_items");
            
            builder.HasMany(c => c.PurchasedItems)
                   .WithOne()
                   .HasForeignKey("ShoppingCartId")
                   .OnDelete(DeleteBehavior.Cascade);
            
            // Configure backing field for PurchasedItems collection
            var purchasedNav = builder.Metadata.FindNavigation(nameof(ShoppingCart.PurchasedItems));
            purchasedNav.SetPropertyAccessMode(PropertyAccessMode.Field);
            purchasedNav.SetField("_purchasedItems");
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.TourId).IsRequired();
        });

        modelBuilder.Entity<PurchasedItem>(builder =>
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.UserId).IsRequired();
            builder.Property(pi => pi.TourId).IsRequired();
            builder.Property(pi => pi.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(pi => pi.AdventureCoinsSpent).IsRequired();
            builder.Property(pi => pi.PurchaseDate).IsRequired();
        });

        modelBuilder.Entity<Coupon>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(8);
            builder.HasIndex(c => c.Code).IsUnique();
            builder.Property(c => c.DiscountPercentage).IsRequired();
            builder.Property(c => c.ExpiryDate);
            builder.Property(c => c.TourId);
            builder.Property(c => c.AuthorId).IsRequired();
        });
    }
}
