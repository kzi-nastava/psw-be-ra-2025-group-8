using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database;

public class PaymentsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PurchasedItem> PurchasedItems { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<BundlePurchaseRecord> BundlePurchaseRecords { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleTour> SaleTours { get; set; }

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
            builder.Property(oi => oi.OriginalPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(oi => oi.DiscountedPrice).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.IsBundle).IsRequired();
            builder.Property(oi => oi.BundleId);
            builder.Property(oi => oi.CouponId);
            builder.Property(oi => oi.SaleId);
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

        modelBuilder.Entity<BundlePurchaseRecord>(builder =>
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.TouristId).IsRequired();
            builder.Property(r => r.BundleId).IsRequired();
            builder.Property(r => r.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(r => r.AdventureCoinsSpent).IsRequired();
            builder.Property(r => r.PurchaseDate).IsRequired();
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

        modelBuilder.Entity<Sale>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.AuthorId).IsRequired();
            builder.Property(s => s.StartDate).IsRequired();
            builder.Property(s => s.EndDate).IsRequired();
            builder.Property(s => s.DiscountPercentage).IsRequired();
            
            builder.HasMany(s => s.SaleTours)
                .WithOne()
                .HasForeignKey(st => st.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
            
            var saleToursNav = builder.Metadata.FindNavigation(nameof(Sale.SaleTours));
            saleToursNav?.SetPropertyAccessMode(PropertyAccessMode.Field);
            saleToursNav?.SetField("_saleTours");
        });

        modelBuilder.Entity<SaleTour>(builder =>
        {
            builder.HasKey(st => st.Id);
            builder.Property(st => st.Id).ValueGeneratedOnAdd();
            builder.Property(st => st.SaleId).IsRequired();
            builder.Property(st => st.TourId).IsRequired();
            builder.HasIndex(st => new { st.SaleId, st.TourId }).IsUnique();
        });
    }
}
