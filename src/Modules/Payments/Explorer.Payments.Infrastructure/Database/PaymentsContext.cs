using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database;

public class PaymentsContext : DbContext
{
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PurchasedItem> PurchasedItems { get; set; }
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
            builder.HasMany(c => c.PurchasedItems)
                   .WithOne()
                   .HasForeignKey("ShoppingCartId")
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.TourId).IsRequired();
        });

        modelBuilder.Entity<PurchasedItem>(builder =>
        {
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.TourId).IsRequired();
            builder.Property(pi => pi.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(pi => pi.PurchaseDate).IsRequired();
        });
    }
}
