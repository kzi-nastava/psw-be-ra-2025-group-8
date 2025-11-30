using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Monument> Monument { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<ReportProblem> ReportProblem { get; set; }
    public DbSet<PersonEquipment> PersonEquipment { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        // Tour Entity Configuration
        modelBuilder.Entity<Tour>().HasKey(t => t.Id);
        modelBuilder.Entity<Tour>().Property(t => t.Name).IsRequired().HasMaxLength(255);
        modelBuilder.Entity<Tour>().Property(t => t.Description).IsRequired();
        modelBuilder.Entity<Tour>().Property(t => t.Difficulty).IsRequired();
        modelBuilder.Entity<Tour>().Property(t => t.Status).IsRequired();
        modelBuilder.Entity<Tour>().Property(t => t.Price).HasColumnType("decimal(18,2)").IsRequired();
        modelBuilder.Entity<Tour>().Property(t => t.AuthorId).IsRequired();
        modelBuilder.Entity<Tour>()
           .Property(t => t.Tags)
           .HasColumnType("text[]");

        modelBuilder.Ignore<Person>();

        modelBuilder.Entity<PersonEquipment>(builder =>
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Equipment)
                   .WithMany()
                   .HasForeignKey(pe => pe.EquipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ShoppingCart>(builder =>
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.UserId).IsRequired();
            builder.HasMany(c => c.Items)
                   .WithOne()
                   .HasForeignKey("ShoppingCartId")
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(c => c.TotalPrice);
        });

        modelBuilder.Entity<OrderItem>(builder =>
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.TourId).IsRequired();
            builder.Property(oi => oi.TourName).IsRequired().HasMaxLength(255);
            builder.Property(oi => oi.Price).HasColumnType("decimal(18,2)").IsRequired();
        });
    }
}