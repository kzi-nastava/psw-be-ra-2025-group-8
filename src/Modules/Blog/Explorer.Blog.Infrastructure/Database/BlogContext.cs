using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database
{
    public class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }
        public DbSet<BlogVote> BlogVotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("blog");

            modelBuilder.Entity<BlogPost>()
                .HasMany(p => p.Votes)
                .WithOne()
                .HasForeignKey(v => v.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogVote>()
                .Property(v => v.Value)
                .IsRequired();

            modelBuilder.Entity<BlogPost>()
                .Property(p => p.Score)
                .IsRequired();

            modelBuilder.Entity<BlogPost>()
                .Property(p => p.IsClosed)
                .IsRequired();
        }
    }
}
