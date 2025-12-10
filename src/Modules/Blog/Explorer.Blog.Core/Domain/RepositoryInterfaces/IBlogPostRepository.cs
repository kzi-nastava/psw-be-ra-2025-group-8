using System.Collections.Generic;

namespace Explorer.Blog.Core.Domain.RepositoryInterfaces;

public interface IBlogPostRepository
{
    BlogPost? Get(long id);
    IEnumerable<BlogPost> GetForAuthor(long authorId);
    IEnumerable<BlogPost> GetPublishedAndArchived();
    IEnumerable<BlogPost> GetActive();
    IEnumerable<BlogPost> GetFamous();
    void Add(BlogPost blogPost);
    void Update(BlogPost blogPost);
    void Delete(long id);
    BlogPost? GetByCommentId(long commentId);
    void RemoveComment(long commentId);
}
