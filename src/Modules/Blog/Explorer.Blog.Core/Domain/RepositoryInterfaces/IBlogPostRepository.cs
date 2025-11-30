using System.Collections.Generic;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogPostRepository
{
    BlogPost? Get(long id);
    IEnumerable<BlogPost> GetForAuthor(long authorId);
    void Add(BlogPost blogPost);
    void Update(BlogPost blogPost);
}
