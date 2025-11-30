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

    //Preference
    public DbSet<TouristPreferences> TouristPreferences { get; set; }
    public DbSet<TransportTypePreferences> TransportTypePreferences { get; set; }
    public DbSet<PreferenceTags> PreferenceTags { get; set; }
    public DbSet<Tags> Tags { get; set; }

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

        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);

        //modelBuilder.Entity<TouristPreferences>()
        //    .HasOne(tp => tp.Person)
        //    .WithOne()
        //    .HasForeignKey<TouristPreferences>(tp => tp.PersonId)
        //    .OnDelete(DeleteBehavior.Cascade);

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
        modelBuilder.Entity<Rating>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId);
    }
}