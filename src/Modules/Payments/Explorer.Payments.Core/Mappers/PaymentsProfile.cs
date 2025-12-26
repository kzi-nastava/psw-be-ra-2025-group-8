using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Mappers;

public class PaymentsProfile : Profile
{
    public PaymentsProfile()
    {
        //mapper za shopping cart i order item
        CreateMap<ShoppingCart, ShoppingCartDto>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
               .ForMember(dest => dest.PurchasedItems, opt => opt.MapFrom(src => src.PurchasedItems));
        CreateMap<OrderItem, OrderItemDto>().ReverseMap()
                .ConstructUsing(dto => new OrderItem(dto.TourId));
        CreateMap<PurchasedItem, PurchasedItemDto>();
    }
}