using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<PersonEquipment> PersonEquipment { get; set; }
    public DbSet<ReportProblem> ReportProblem { get; set; }
    public DbSet<Tour> Tours { get; set; }

    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Ignore<Person>();

        modelBuilder.Entity<PersonEquipment>(builder =>
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Equipment)
                   .WithMany()
                   .HasForeignKey(pe => pe.EquipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

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
    }
}