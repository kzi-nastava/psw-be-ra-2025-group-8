using Explorer.Encounters.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database;

public class EncountersContext : DbContext
{
    public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }

    public DbSet<Encounter> Encounters { get; set; }
    public DbSet<EncounterParticipation> EncounterParticipations { get; set; }
    public DbSet<Challenge> Challenges { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("encounters");

        // Encounter configuration
        modelBuilder.Entity<Encounter>(entity =>
        {
            entity.ToTable("Encounters");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Description)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.Location)
                .IsRequired(false);

            entity.Property(e => e.Latitude)
                .IsRequired(false);

            entity.Property(e => e.Longitude)
                .IsRequired(false);

            entity.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.Type)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.XPReward)
                .IsRequired()
                .HasColumnType("integer");

            entity.Property(e => e.PublishedAt)
                .IsRequired(false);

            entity.Property(e => e.ArchivedAt)
                .IsRequired(false);

            // Creator
            entity.Property(e => e.CreatorPersonId)
                .IsRequired(false)
                .HasColumnType("bigint");

            // Social settings
            entity.Property(e => e.SocialRequiredCount)
                .IsRequired(false)
                .HasColumnType("integer");

            entity.Property(e => e.SocialRangeMeters)
                .IsRequired(false)
                .HasColumnType("double precision");
        });

        // Challenge configuration
        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.ToTable("Challenges");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasColumnType("text");
            entity.Property(c => c.Description).IsRequired(false).HasColumnType("text");
            entity.Property(c => c.Latitude).IsRequired();
            entity.Property(c => c.Longitude).IsRequired();
            entity.Property(c => c.CreatorPersonId).IsRequired();
            entity.Property(c => c.Status).HasConversion<int>().IsRequired();
            entity.Property(c => c.XPReward).IsRequired().HasColumnType("integer");
            entity.Property(c => c.CreatedAt).IsRequired();
            entity.Property(c => c.ApprovedAt).IsRequired(false);
        });

        // EncounterParticipation configuration
        modelBuilder.Entity<EncounterParticipation>(entity =>
        {
            entity.ToTable("EncounterParticipations");

            // Composite primary key - ensures one participation per person per encounter
            entity.HasKey(p => new { p.PersonId, p.EncounterId });

            entity.Property(p => p.PersonId)
                .IsRequired();

            entity.Property(p => p.EncounterId)
                .IsRequired();

            entity.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            entity.Property(p => p.ActivatedAt)
                .IsRequired();

            entity.Property(p => p.CompletedAt)
                .IsRequired(false);

            entity.Property(p => p.XPAwarded)
                .IsRequired(false)
                .HasColumnType("integer");

            // Indexes for faster queries
            entity.HasIndex(p => p.PersonId);
            entity.HasIndex(p => p.EncounterId);
            entity.HasIndex(p => new { p.PersonId, p.Status });
        });
    }
}