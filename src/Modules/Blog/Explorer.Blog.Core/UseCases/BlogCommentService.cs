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
    private readonly IInternalPersonService _personService;
    private readonly IInternalUserService _userService;
    private readonly IMapper _mapper;

    public BlogCommentService(IBlogPostRepository blogPostRepository, IInternalPersonService personService, IInternalUserService userService, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _personService = personService;
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
            EnrichCommentWithUserInfo(commentDto);
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

        var commentDtos = _mapper.Map<List<CommentDto>>(
            blogPost.Comments.OrderByDescending(c => c.CreationTime).ToList()
        );
        
        EnrichCommentsWithUserInfo(commentDtos);
        
        return commentDtos;
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
            EnrichCommentWithUserInfo(commentDto);
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

    private void EnrichCommentWithUserInfo(CommentDto commentDto)
    {
        if (commentDto == null) return;
        
        try
        {
            var person = _personService.GetByUserId(commentDto.PersonId);
            var user = _userService.GetById(commentDto.PersonId);
            
            commentDto.PersonName = person?.Name ?? "";
            commentDto.PersonSurname = person?.Surname ?? "";
            commentDto.PersonUsername = user?.Username ?? "";
        }
        catch { }
    }

    private void EnrichCommentsWithUserInfo(List<CommentDto> comments)
    {
        if (comments == null || !comments.Any()) return;

        var personIds = comments.Select(c => c.PersonId).Distinct().ToList();
        var personData = _personService.GetByUserIds(personIds);
        var userData = _userService.GetByIds(personIds);

        foreach (var comment in comments)
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