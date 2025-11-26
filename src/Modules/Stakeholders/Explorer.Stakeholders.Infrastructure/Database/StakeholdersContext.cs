using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Meetup> Meetups { get; set; }
    public DbSet<Club> Clubs { get; set; }

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

        modelBuilder.Entity<Club>(b =>
        {
            b.ToTable("Clubs");
            b.HasKey(c => c.Id);
            b.HasOne<User>()
                .WithMany()
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);
            //konverzije za slike kluba
            var imagesConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

            b.Property(c => c.ImageUrls)
             .HasConversion(imagesConverter)
             .HasColumnType("text");
            //konverzije za clanove kluba
            var membersConverter = new ValueConverter<List<long>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<long>>(v, (JsonSerializerOptions?)null) ?? new List<long>());

            b.Property(c => c.MemberIds)
                .HasConversion(membersConverter)
                .HasColumnType("text");
        });


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
