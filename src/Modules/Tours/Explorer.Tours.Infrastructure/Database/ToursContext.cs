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

        // TOUR CONFIGURATION
        modelBuilder.Entity<Tour>(builder =>
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(t => t.Description)
                .IsRequired();

            builder.Property(t => t.Difficulty)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired();

            builder.Property(t => t.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.AuthorId)
                .IsRequired();



            // route length
            builder.Property(t => t.LengthInKilometers)
                .HasColumnType("double precision")
                .HasDefaultValue(0.0);  // important because of c-tours.sql insert

            // Tour -> KeyPoints (1 - N)
            builder.HasMany(t => t.KeyPoints)
                .WithOne()                         // KeyPoint does not have navigation property to Tour
                .HasForeignKey("TourId")           // shadow FK column TourId
                .OnDelete(DeleteBehavior.Cascade); // deleting KeyPoints when Tour is deleted
        });

        // TourExecution Entity Configuration
        modelBuilder.Entity<TourExecution>().HasKey(te => te.Id);
        modelBuilder.Entity<TourExecution>().Property(te => te.IdTour).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.Longitude).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.Latitude).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.IdTourist).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.Status).IsRequired();
        modelBuilder.Entity<TourExecution>().Property(te => te.LastActivity).IsRequired();

        // KEYPOINT CONFIGURATION
        modelBuilder.Entity<KeyPoint>(builder =>
        {
            builder.HasKey(kp => kp.Id);

            builder.Property(kp => kp.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(kp => kp.Description)
                .IsRequired(false);

            builder.Property(kp => kp.ImageUrl)
                .IsRequired(false);

            builder.Property(kp => kp.Secret)
                .IsRequired(false);

            builder.Property(kp => kp.Order)
                .IsRequired();

            builder.OwnsOne(kp => kp.Location, navigation =>
            {
                navigation.Property(c => c.Latitude)
                    .HasColumnName("Latitude")
                    .IsRequired();

                navigation.Property(c => c.Longitude)
                    .HasColumnName("Longitude")
                    .IsRequired();
            });
        });

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