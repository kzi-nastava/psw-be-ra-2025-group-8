using AutoMapper;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogImage, BlogImageDto>();

        CreateMap<BlogPost, BlogPostDto>();

        CreateMap<CreateBlogPostDto, BlogPost>();
        CreateMap<UpdateBlogPostDto, BlogPost>();
    }
}
