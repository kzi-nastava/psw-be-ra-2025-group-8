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
    public DbSet<IssueMessage> IssueMessages { get; set; }
    public DbSet<PersonEquipment> PersonEquipment { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<TourEquipment> TourEquipment { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<TourExecution> TourExecutions { get; set; }
    public DbSet<KeyPointReached> KeyPointsReached { get; set; }
    public DbSet<KeyPoint> KeyPoints { get; set; }
    public DbSet<TourTransportTime> TourTransportTimes { get; set; }


    //Preference
    public DbSet<TouristPreferences> TouristPreferences { get; set; }
    public DbSet<TransportTypePreferences> TransportTypePreferences { get; set; }
    public DbSet<PreferenceTags> PreferenceTags { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<TourTag> TourTags { get; set; }


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

            builder.Property(t => t.PublishedAt)
                .IsRequired(false);

            builder.Property(t => t.ArchivedAt)
                .IsRequired(false);

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

        // TourTransportTime CONFIGURATION
        modelBuilder.Entity<TourTransportTime>(builder =>
        {
            builder.HasKey(tt => new { tt.TourId, tt.Transport });

            builder.Property(tt => tt.DurationMinutes)
                .IsRequired();

            builder.HasOne(tt => tt.Tour)
                .WithMany(t => t.TransportTimes)
                .HasForeignKey(tt => tt.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TourTag>()
            .HasKey(tt => new { tt.TourId, tt.TagsId });

        modelBuilder.Entity<TourTag>()
            .HasOne(tt => tt.Tour)
            .WithMany(t => t.TourTags)
            .HasForeignKey(tt => tt.TourId)
            .OnDelete(DeleteBehavior.Cascade);  // tags are not deleted when tour is deleted

        modelBuilder.Entity<TourTag>()
            .HasOne(tt => tt.Tags)
            .WithMany(t => t.TourTags)
            .HasForeignKey(tt => tt.TagsId)
            .OnDelete(DeleteBehavior.Cascade);  // if tag is deleted (by admin), remove it from all tours

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



        modelBuilder.Ignore<Person>();

        modelBuilder.Entity<PersonEquipment>(builder =>
        {
            builder.HasKey(pe => pe.Id);

            builder.HasOne(pe => pe.Equipment)
                   .WithMany()
                   .HasForeignKey(pe => pe.EquipmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TouristPreferences>()
            .HasMany(tp => tp.TransportTypePreferences)
            .WithOne(t => t.Preference)
            .HasForeignKey(t => t.PreferenceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TransportTypePreferences>()
            .HasOne(t => t.Preference)
            .WithMany(p => p.TransportTypePreferences)
            .HasForeignKey(t => t.PreferenceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PreferenceTags>()
        .HasKey(pt => new { pt.TouristPreferencesId, pt.TagsId });

        modelBuilder.Entity<PreferenceTags>()
            .HasOne(pt => pt.TouristPreferences)
            .WithMany(tp => tp.PreferenceTags)
            .HasForeignKey(pt => pt.TouristPreferencesId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PreferenceTags>()
            .HasOne(pt => pt.Tags)
            .WithMany(t => t.PreferenceTags)
            .HasForeignKey(pt => pt.TagsId)
            .OnDelete(DeleteBehavior.Cascade);

        //enum konverzije
        modelBuilder.Entity<TouristPreferences>()
            .Property(tp => tp.Difficulty)
            .HasConversion<string>();

        modelBuilder.Entity<TransportTypePreferences>()
            .Property(t => t.Transport)
            .HasConversion<string>();

        modelBuilder.Entity<TourEquipment>()
            .HasKey(te => new { te.TourId, te.EquipmentId });

        modelBuilder.Entity<TourEquipment>()
            .HasOne(te => te.Tour)
            .WithMany(t => t.RequiredEquipment)
            .HasForeignKey(te => te.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TourEquipment>()
            .HasOne(te => te.Equipment)
            .WithMany()
            .HasForeignKey(te => te.EquipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // ReportProblem <-> IssueMessage (1:N)
        modelBuilder.Entity<ReportProblem>()
            .HasMany(rp => rp.Messages)
            .WithOne()
            .HasForeignKey(m => m.ReportProblemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IssueMessage>()
            .HasKey(m => m.Id);
        modelBuilder.Entity<IssueMessage>()
            .Property(m => m.Content).IsRequired();
        modelBuilder.Entity<IssueMessage>()
            .Property(m => m.AuthorId).IsRequired();
        modelBuilder.Entity<IssueMessage>()
            .Property(m => m.CreatedAt).IsRequired();
    }

}