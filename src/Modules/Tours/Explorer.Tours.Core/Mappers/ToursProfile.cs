using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class ToursProfile : Profile
{
    public ToursProfile()
    {
        CreateMap<EquipmentDto, Equipment>().ReverseMap();
        CreateMap<MonumentDto, Monument>().ReverseMap();
        CreateMap<ReportProblemDto, ReportProblem>().ReverseMap();
        CreateMap<FacilityDto, Facility>().ReverseMap();

        CreateMap<TourDto, Tour>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapStatus(src.Status)));

        CreateMap<Tour, TourDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<ShoppingCart, ShoppingCartDto>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
               .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
               .ReverseMap()
               .ConstructUsing(dto => new ShoppingCart(dto.UserId));
        CreateMap<OrderItem, OrderItemDto>().ReverseMap()
                .ConstructUsing(dto => new OrderItem(dto.TourId, dto.TourName, dto.Price));
    }

    private static TourStatus MapStatus(string status)
    {
        if (Enum.TryParse<TourStatus>(status, out var parsed)) return parsed;
        return TourStatus.Draft;
    }
}