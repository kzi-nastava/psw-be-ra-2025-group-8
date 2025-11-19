using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Meetup> Meetups { get; set; }

    //Preference
    public DbSet<TouristPreferences> TouristPreferences { get; set; }
    public DbSet<TransportTypePreferences> TransportTypePreferences { get; set; }
    public DbSet<PreferenceTags> PreferenceTags { get; set; }
    public DbSet<Tags> Tags { get; set; }


    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);

        modelBuilder.Entity<TouristPreferences>()
            .HasOne(tp => tp.Person)
            .WithOne()
            .HasForeignKey<TouristPreferences>(tp => tp.PersonId)
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
    }
}
