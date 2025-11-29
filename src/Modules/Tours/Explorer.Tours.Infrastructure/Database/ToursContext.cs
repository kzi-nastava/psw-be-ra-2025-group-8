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
    public DbSet<TourExecution> TourExecutions { get; set; }
    public DbSet<KeyPointReached> KeyPointsReached { get; set; }
    public DbSet<KeyPoint> KeyPoints { get; set; }
    
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
        modelBuilder.Entity<Tour>().Property(t => t.Tags).HasColumnType("text[]");

        // TourExecution Entity Configuration
        modelBuilder.Entity<TourExecution>().HasKey(te => te.Id);
        modelBuilder.Entity<TourExecution>().Property(te => te.IdTour).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.Longitude).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.Latitude).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.IdTourist).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.Status).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.LastActivity).IsRequired();

        // TourKeyPoint Entity Configuration
        modelBuilder.Entity<KeyPoint>().HasKey(tkp => tkp.Id);
        modelBuilder.Entity<KeyPoint>().Property(tkp => tkp.TourId).IsRequired();
        modelBuilder.Entity<KeyPoint>().Property(tkp => tkp.OrderNum).IsRequired();
        modelBuilder.Entity<KeyPoint>().Property(tkp => tkp.Latitude).IsRequired();
        modelBuilder.Entity<KeyPoint>().Property(tkp => tkp.Longitude).IsRequired();
        
        // Relationship: Tour has many KeyPoints
        modelBuilder.Entity<KeyPoint>().HasOne<Tour>().WithMany().HasForeignKey(tkp => tkp.TourId).OnDelete(DeleteBehavior.Cascade);

        // KeyPointReached Entity Configuration
        modelBuilder.Entity<KeyPointReached>().HasKey(kpr => kpr.Id);
        modelBuilder.Entity<KeyPointReached>().Property(kpr => kpr.TourExecutionId).IsRequired();
        modelBuilder.Entity<KeyPointReached>().Property(kpr => kpr.KeyPointOrder).IsRequired();
        modelBuilder.Entity<KeyPointReached>().Property(kpr => kpr.ReachedAt).IsRequired();
        modelBuilder.Entity<KeyPointReached>().Property(kpr => kpr.Latitude).IsRequired();
        modelBuilder.Entity<KeyPointReached>().Property(kpr => kpr.Longitude).IsRequired();
        
        // Relationship: TourExecution has many KeyPointReached
        modelBuilder.Entity<KeyPointReached>().HasOne<TourExecution>().WithMany().HasForeignKey(kpr => kpr.TourExecutionId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Ignore<Person>();

        modelBuilder.Entity<PersonEquipment>(builder =>
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Equipment)
                   .WithMany()
                   .HasForeignKey(pe => pe.EquipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        });
    }
}