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

    public BlogPostDto Update(long id, UpdateBlogPostDto request)
    {
        var blogPost = _blogPostRepository.Get(id);
        if (blogPost == null) throw new KeyNotFoundException("Blog post not found.");

        var images = request.Images?
            .Select(i => new BlogImage(i.Url, i.Order));

        blogPost.Edit(request.Title, request.Description, images);
        _blogPostRepository.Update(blogPost);

        return _mapper.Map<BlogPostDto>(blogPost);
    }

    public List<BlogPostDto> GetForAuthor(long authorId)
    {
        var blogPosts = _blogPostRepository.GetForAuthor(authorId);
        return _mapper.Map<List<BlogPostDto>>(blogPosts.ToList());
    }
}
