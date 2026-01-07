using Explorer.Encounters.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database;

public class EncountersContext : DbContext
{
    public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }
    public DbSet<Encounter> Encounters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("encounters");

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
        });
    }
}
