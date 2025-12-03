using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; }
    public DbSet<BlogImage> BlogImages { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        modelBuilder.Entity<BlogPost>()
            .Property(b => b.Status)
            .HasConversion<int>();

        // osiguravamo kaskadno brisanje komentara
        modelBuilder.Entity<BlogPost>()
            .HasMany(b => b.Comments)
            .WithOne()
            .HasForeignKey("BlogPostId")
            .OnDelete(DeleteBehavior.Cascade);

        // mapiranje comment entiteta
        modelBuilder.Entity<Comment>().ToTable("Comments");
    }
}
