using AutoMapper;
using Explorer.Blog.Core.Domain;
using Explorer.Blog.API.Dtos;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogImage, BlogImageDto>();

        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.PopularityStatus, opt => opt.MapFrom(src => (int)src.PopularityStatus));

        CreateMap<CreateBlogPostDto, BlogPost>();
        CreateMap<UpdateBlogPostDto, BlogPost>();

        CreateMap<Comment, CommentDto>();

        CreateMap<Vote, VoteDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));
    }
}