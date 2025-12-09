using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogImage> BlogImages { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Vote> Votes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.Entity<BlogPost>()
            .Property(b => b.Status)
            .HasConversion<int>();

        // cascade deletion of comments
        modelBuilder.Entity<BlogPost>()
            .HasMany(b => b.Comments)
            .WithOne()
            .HasForeignKey("BlogPostId")
            .OnDelete(DeleteBehavior.Cascade);

        // cascade deletion of votes
        modelBuilder.Entity<BlogPost>()
            .HasMany(b => b.Votes)
            .WithOne()
            .HasForeignKey("BlogPostId")
            .OnDelete(DeleteBehavior.Cascade);

        // mapping comment entity
        modelBuilder.Entity<Comment>().ToTable("Comments");

        // mapping vote entity
        modelBuilder.Entity<Vote>()
            .ToTable("Votes")
            .Property(v => v.Type)
            .HasConversion<int>();
    }
}
