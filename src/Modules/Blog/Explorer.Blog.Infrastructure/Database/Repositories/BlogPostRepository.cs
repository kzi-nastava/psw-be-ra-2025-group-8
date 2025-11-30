using System.Collections.Generic;
using System.Linq;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Blog.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly BlogContext _dbContext;

    public BlogPostRepository(BlogContext dbContext)
    {
        _dbContext = dbContext;
    }

    public BlogPost? Get(long id)
    {
        return _dbContext.Set<BlogPost>()
            .Include(b => b.Images)
            .FirstOrDefault(b => b.Id == id);
    }

    public IEnumerable<BlogPost> GetForAuthor(long authorId)
    {
        return _dbContext.Set<BlogPost>()
            .Include(b => b.Images)
            .Where(b => b.AuthorId == authorId)
            .ToList();
    }

    public void Add(BlogPost blogPost)
    {
        _dbContext.Set<BlogPost>().Add(blogPost);
        _dbContext.SaveChanges();
    }

    public void Update(BlogPost blogPost)
    {
        _dbContext.Set<BlogPost>().Update(blogPost);
        _dbContext.SaveChanges();
    }

    public void Delete(long id)
    {
        var blogPost = _dbContext.BlogPosts
            .Include(bp => bp.Images)
            .FirstOrDefault(bp => bp.Id == id);

        if (blogPost == null) return;

        // EF će automatski obrisati slike (zbog FK i cascade rules)
        _dbContext.BlogPosts.Remove(blogPost);
        _dbContext.SaveChanges();
    }
}
