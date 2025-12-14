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
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<FollowerMessage> FollowerMessages { get; set; }


    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        
        modelBuilder.Entity<Notification>().HasIndex(n => n.UserId);
  
        // Follower indexes
        modelBuilder.Entity<Follower>().HasIndex(f => f.UserId);
        modelBuilder.Entity<Follower>().HasIndex(f => f.FollowingUserId);
        modelBuilder.Entity<Follower>().HasIndex(f => new { f.UserId, f.FollowingUserId }).IsUnique();

        // FollowerMessage indexes
        modelBuilder.Entity<FollowerMessage>().HasIndex(m => m.SenderId);

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);


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


        modelBuilder.Entity<Rating>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId);
    }
}
