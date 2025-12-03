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

    // Repozitorijum i Maper se ubacuju preko DI
    public BlogCommentService(IBlogPostRepository blogPostRepository, IMapper mapper)
    {
        _blogPostRepository = blogPostRepository;
        _mapper = mapper;
    }

    public CommentDto Create(long blogId, long personId, CommentCreationDto commentData)
    {
        // 1. Učitavanje Blog agregata (uključuje i postojeće komentare zahvaljujući Koraku 2)
        var blogPost = _blogPostRepository.Get(blogId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog Post with ID {blogId} not found.");

        try
        {
            // 2. Pozivanje domenske logike na agregatu
            var newComment = blogPost.AddComment(
                personId,
                commentData.Text
            );

            // 3. Čuvanje agregata preko repozitorijuma (EFC će sačuvati i novi komentar)
            _blogPostRepository.Update(blogPost);

            // 4. Mapiranje i vraćanje DTO-a
            return _mapper.Map<CommentDto>(newComment);
        }
        catch (InvalidOperationException e)
        {
            // Hvatanje domenske validacije (npr. Blog nije objavljen)
            throw new UnauthorizedAccessException(e.Message);
        }
        catch (ArgumentException)
        {
            // Hvatanje domenske validacije (npr. prazan tekst)
            throw; // Ponovno bacanje (ovo Kontroler treba da uhvati i vrati 400)
        }
    }

    public List<CommentDto> GetCommentsForBlog(long blogId)
    {
        // 1. Učitavanje Blog agregata
        var blogPost = _blogPostRepository.Get(blogId);

        if (blogPost == null)
            throw new KeyNotFoundException($"Blog Post with ID {blogId} not found.");

        // 2. Vraćanje komentara za prikaz
        return _mapper.Map<List<CommentDto>>(
            blogPost.Comments.OrderByDescending(c => c.CreationTime).ToList()
        );
    }
}