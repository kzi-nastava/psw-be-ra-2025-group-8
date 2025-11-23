using Explorer.Clubs.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Explorer.Clubs.Infrastructure.Database
{
    public class ClubsContext : DbContext
    {
        public DbSet<Club> Clubs { get; set; }

        public ClubsContext(DbContextOptions<ClubsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("clubs");

            modelBuilder.Entity<Club>(b =>
            {
                b.ToTable("Clubs");
                b.HasKey(c => c.Id);

                var imagesConverter = new ValueConverter<List<string>, string>(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

                b.Property(c => c.ImageUrls)
                 .HasConversion(imagesConverter)
                 .HasColumnType("text");

                var membersConverter = new ValueConverter<List<long>, string>(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<long>>(v, (JsonSerializerOptions?)null) ?? new List<long>());

                b.Property(c => c.MemberIds)
                    .HasConversion(membersConverter)
                    .HasColumnType("text");
            });




        }
    }
}
