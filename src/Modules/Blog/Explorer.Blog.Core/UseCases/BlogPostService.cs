using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Blog.Core.UseCases;

public class BlogPostService : IBlogPostService
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IInternalPersonService _personService;
    private readonly IInternalUserService _userService;
    private readonly IMapper _mapper;

    public BlogPostService(IBlogPostRepository blogPostRepository, IInternalPersonService personService, IInternalUserService userService, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _personService = personService;
        _userService = userService;
        _mapper = mapper;
    }

    public BlogPostDto Create(CreateBlogPostDto request)
    {
        var images = request.Images?
            .Select(i => new BlogImage(i.Url, i.Order));

        var blogPost = new BlogPost(request.AuthorId, request.Title, request.Description, images);
        _blogPostRepository.Add(blogPost);

        return MapBlogPostDto(blogPost, request.AuthorId);
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

        return MapBlogPostDto(blogPost, authorId);
    }

    public BlogPostDto UpdatePublished(long id, UpdatePublishedBlogPostDto request, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only update your own blog posts.");

        blogPost.UpdatePublished(request.Description);
        _blogPostRepository.Update(blogPost);

        return MapBlogPostDto(blogPost, authorId);
    }

    public BlogPostDto Publish(long id, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only publish your own blog posts.");

        blogPost.Publish();
        _blogPostRepository.Update(blogPost);

        return MapBlogPostDto(blogPost, authorId);
    }

    public BlogPostDto Archive(long id, long authorId)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");
        if (blogPost.AuthorId != authorId) throw new UnauthorizedAccessException("You can only archive your own blog posts.");

        blogPost.Archive();
        _blogPostRepository.Update(blogPost);

        return MapBlogPostDto(blogPost, authorId);
    }

    public List<BlogPostDto> GetForAuthor(long authorId)
    {
        var blogPosts = _blogPostRepository.GetForAuthor(authorId);
        return blogPosts.Select(bp => MapBlogPostDto(bp, authorId)).ToList();
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
            return allVisible.Select(bp => MapBlogPostDto(bp, userId)).ToList();
        }

        var publishedAndArchivedAnonymous = _blogPostRepository.GetPublishedAndArchived().ToList();
        return publishedAndArchivedAnonymous.Select(bp => MapBlogPostDto(bp, null)).ToList();
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

        return MapBlogPostDto(blogPost, userId);
    }

    public void Delete(long id)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");

        _blogPostRepository.Delete(id);
    }

    public BlogPostDto AddUpvote(long blogPostId, long personId)
    {
        var blogPost = _blogPostRepository.Get(blogPostId);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");

        blogPost.AddVote(personId, VoteType.Upvote);
        _blogPostRepository.Update(blogPost);

        return MapBlogPostDto(blogPost, personId);
    }

    public BlogPostDto AddDownvote(long blogPostId, long personId)
    {
        var blogPost = _blogPostRepository.Get(blogPostId);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");

        blogPost.AddVote(personId, VoteType.Downvote);
        _blogPostRepository.Update(blogPost);

        return MapBlogPostDto(blogPost, personId);
    }

    public List<BlogPostDto> GetActive(long? userId)
    {
        var activeBlogs = _blogPostRepository.GetActive().ToList();
        return activeBlogs.Select(bp => MapBlogPostDto(bp, userId)).ToList();
    }

    public List<BlogPostDto> GetFamous(long? userId)
    {
        var famousBlogs = _blogPostRepository.GetFamous().ToList();
        return famousBlogs.Select(bp => MapBlogPostDto(bp, userId)).ToList();
    }

    private BlogPostDto MapBlogPostDto(BlogPost blogPost, long? userId)
    {
        var dto = _mapper.Map<BlogPostDto>(blogPost);
        dto.UpvoteCount = blogPost.GetUpvoteCount();
        dto.DownvoteCount = blogPost.GetDownvoteCount();
        
        if (userId.HasValue)
        {
            var userVote = blogPost.GetUserVote(userId.Value);
            if (userVote != null)
            {
                dto.UserVote = _mapper.Map<VoteDto>(userVote);
            }
        }

        // Enrich with author information
        EnrichBlogPostWithUserInfo(dto);

        return dto;
    }

    private void EnrichBlogPostWithUserInfo(BlogPostDto blogPostDto)
    {
        if (blogPostDto == null) return;

        // Enrich author info
        try
        {
            var authorPerson = _personService.GetByUserId(blogPostDto.AuthorId);
            var authorUser = _userService.GetById(blogPostDto.AuthorId);
            
            blogPostDto.AuthorName = authorPerson?.Name ?? "";
            blogPostDto.AuthorSurname = authorPerson?.Surname ?? "";
            blogPostDto.AuthorUsername = authorUser?.Username ?? "";
        }
        catch { }

        // Enrich comments with person info
        if (blogPostDto.Comments != null && blogPostDto.Comments.Any())
        {
            var personIds = blogPostDto.Comments.Select(c => c.PersonId).Distinct().ToList();
            var personData = _personService.GetByUserIds(personIds);
            var userData = _userService.GetByIds(personIds);

            foreach (var comment in blogPostDto.Comments)
            {
                if (personData.TryGetValue(comment.PersonId, out var person))
                {
                    comment.PersonName = person.Name;
                    comment.PersonSurname = person.Surname;
                }
                
                if (userData.TryGetValue(comment.PersonId, out var user))
                {
                    comment.PersonUsername = user.Username;
                }
            }
        }
    }
}
