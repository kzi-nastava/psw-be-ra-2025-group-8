using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;

namespace Explorer.Blog.Core.UseCases;

public class BlogCommentService : IBlogCommentService
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IMapper _mapper;

    public BlogCommentService(IBlogPostRepository blogPostRepository, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _mapper = mapper;
    }

    public CommentDto Create(long blogId, long personId, CommentCreationDto commentData)
    {
        // load Blog agregate(with comments)
        var blogPost = _blogPostRepository.Get(blogId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog Post with ID {blogId} not found.");

        try
        {
            // connect domain logic for agregate
            var newComment = blogPost.AddComment(
                personId,
                commentData.Text
            );

            _blogPostRepository.Update(blogPost);

            return _mapper.Map<CommentDto>(newComment);
        }
        catch (InvalidOperationException e)
        {
            // Blog not published
            throw new UnauthorizedAccessException(e.Message);
        }
        catch (ArgumentException)
        {
            // empty text
            throw;
        }
    }

    public List<CommentDto> GetCommentsForBlog(long blogId)
    {
        var blogPost = _blogPostRepository.Get(blogId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog Post with ID {blogId} not found.");

        return _mapper.Map<List<CommentDto>>(
            blogPost.Comments.OrderByDescending(c => c.CreationTime).ToList()
        );
    }
}