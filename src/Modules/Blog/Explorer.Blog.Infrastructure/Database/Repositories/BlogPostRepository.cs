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
            .Include(b => b.Comments)
            .Include(b => b.Votes)
            .FirstOrDefault(b => b.Id == id);
    }

    public IEnumerable<BlogPost> GetForAuthor(long authorId)
    {
        return _dbContext.Set<BlogPost>()
            .Include(b => b.Images)
            .Include(b => b.Comments)
            .Include(b => b.Votes)
            .Where(b => b.AuthorId == authorId)
            .ToList();
    }

    public IEnumerable<BlogPost> GetPublishedAndArchived()
    {
        return _dbContext.Set<BlogPost>()
            .Include(b => b.Images)
            .Include(b => b.Comments)
            .Include(b => b.Votes)
            .Where(b => b.Status == BlogStatus.Published || b.Status == BlogStatus.Archived)
            .ToList();
    }

    public IEnumerable<BlogPost> GetActive()
    {
        return _dbContext.Set<BlogPost>()
            .Include(b => b.Images)
            .Include(b => b.Comments)
            .Include(b => b.Votes)
            .Where(b => b.PopularityStatus == BlogPopularityStatus.Active)
            .ToList();
    }

    public IEnumerable<BlogPost> GetFamous()
    {
        return _dbContext.Set<BlogPost>()
            .Include(b => b.Images)
            .Include(b => b.Comments)
            .Include(b => b.Votes)
            .Where(b => b.PopularityStatus == BlogPopularityStatus.Famous)
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

        _dbContext.BlogPosts.Remove(blogPost);
        _dbContext.SaveChanges();
    }
}
