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
        CreateMap<PurchasedItem, PurchasedItemDto>()
            .ForMember(dest => dest.OriginalPrice, opt => opt.MapFrom(src => src.OriginalPrice))
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.SaleId))
            .ForMember(dest => dest.CouponId, opt => opt.MapFrom(src => src.CouponId));
        CreateMap<OrderItem, OrderItemDto>();

        //za kupovinu bundle-a
        CreateMap<BundlePurchaseRecord, BundlePurchaseRecordDto>();


        //mapper za coupon
        CreateMap<Coupon, CouponDto>();
        CreateMap<CreateCouponDto, Coupon>();
        CreateMap<UpdateCouponDto, Coupon>();

        //mapper za sale
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.TourIds, opt => opt.MapFrom(src => src.TourIds.ToList()));
        CreateMap<CreateSaleDto, Sale>();
        CreateMap<UpdateSaleDto, Sale>();
    }
}