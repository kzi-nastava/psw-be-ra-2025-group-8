using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Explorer.Blog.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Blog.Core.UseCases;

public class BlogCommentService : IBlogCommentService
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly IInternalUserService _userService;
    private readonly IMapper _mapper;

    public BlogCommentService(IBlogPostRepository blogPostRepository, IInternalUserService userService, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _userService = userService;
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

            var commentDto = _mapper.Map<CommentDto>(newComment);
            commentDto.PersonUsername = _userService.GetUsernameById(personId);
            return commentDto;
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

        var comments = _mapper.Map<List<CommentDto>>(
            blogPost.Comments.OrderByDescending(c => c.CreationTime).ToList()
        );

        // Enrich each comment with username
        foreach (var comment in comments)
        {
            comment.PersonUsername = _userService.GetUsernameById(comment.PersonId);
        }

        return comments;
    }

    public CommentDto Update(long userId, long commentId, CommentCreationDto commentData)
    {
        var blogPost = _blogPostRepository.GetByCommentId(commentId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog Post for comment ID {commentId} not found.");

        try
        {
            // domain logic implemeted
            var updatedComment = blogPost.UpdateComment(
                commentId,
                userId,
                commentData.Text
            );

            _blogPostRepository.Update(blogPost);

            var commentDto = _mapper.Map<CommentDto>(updatedComment);
            commentDto.PersonUsername = _userService.GetUsernameById(userId);
            return commentDto;
        }
        catch (KeyNotFoundException)
        {
            // comment not found
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            // not author or passed 15 min threshold
            throw;
        }
        catch (ArgumentException)
        {
            // empty comment
            throw;
        }
    }

    public void Delete(long userId, long commentId)
    {
        var blogPost = _blogPostRepository.GetByCommentId(commentId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog Post for comment ID {commentId} not found.");

        try
        {
            // domain logic implemented
            blogPost.DeleteComment(commentId, userId);
            _blogPostRepository.RemoveComment(commentId);
            _blogPostRepository.Update(blogPost);
        }
        catch (KeyNotFoundException)
        {
            // comment not found
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            // not author or passed 15 min threshold
            throw;
        }
    }
}