using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IMapper _mapper;

    public BlogPostService(IBlogPostRepository blogPostRepository, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _mapper = mapper;
    }

    public BlogPostDto Create(CreateBlogPostDto request)
    {
        var images = request.Images?
            .Select(i => new BlogImage(i.Url, i.Order));

        var blogPost = new BlogPost(request.AuthorId, request.Title, request.Description, images);
        _blogPostRepository.Add(blogPost);

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public BlogPostDto UpdateDraft(long id, UpdateBlogPostDto request, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only update your own blog posts.");

        var images = request.Images?
            .Select(i => new BlogImage(i.Url, i.Order));

        blogPost.UpdateDraft(request.Title, request.Description, images);
        _blogPostRepository.Update(blogPost);

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public BlogPostDto UpdatePublished(long id, UpdatePublishedBlogPostDto request, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only update your own blog posts.");

        blogPost.UpdatePublished(request.Description);
        _blogPostRepository.Update(blogPost);

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public BlogPostDto Publish(long id, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only publish your own blog posts.");

        blogPost.Publish();
        _blogPostRepository.Update(blogPost);

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public BlogPostDto Archive(long id, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only archive your own blog posts.");

        blogPost.Archive();
        _blogPostRepository.Update(blogPost);

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public List<BlogPostDto> GetForAuthor(long authorId)
    {
        var blogPosts = _blogPostRepository.GetForAuthor(authorId);
        return _mapper.Map<List<BlogPostDto>>(blogPosts.ToList());
    }

    public List<BlogPostDto> GetVisibleBlogs(long? userId)
    {
        if (userId.HasValue)
        {
            var authorDrafts = _blogPostRepository.GetForAuthor(userId.Value)
                .Where(b => b.Status == BlogStatus.Draft)
                .ToList();
            
            var publishedAndArchived = _blogPostRepository.GetPublishedAndArchived().ToList();
            
            var allVisible = publishedAndArchived.Concat(authorDrafts).ToList();
            return _mapper.Map<List<BlogPostDto>>(allVisible);
        }

        var publishedAndArchivedAnonymous = _blogPostRepository.GetPublishedAndArchived().ToList();
        return _mapper.Map<List<BlogPostDto>>(publishedAndArchivedAnonymous);
    }

    public BlogPostDto GetById(long id, long? userId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");

        // Check visibility rules
        var isPubliclyVisible = blogPost.Status == BlogStatus.Published || blogPost.Status == BlogStatus.Archived;
        var isOwnDraft = userId.HasValue && blogPost.Status == BlogStatus.Draft && blogPost.AuthorId == userId.Value;

        if (!isPubliclyVisible && !isOwnDraft)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this blog post.");
        }

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public void Delete(long id)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");

        _blogPostRepository.Delete(id);
    }
}
