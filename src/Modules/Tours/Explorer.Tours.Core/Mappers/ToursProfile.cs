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
        CreateMap<IssueMessageDto, IssueMessage>().ReverseMap();
        CreateMap<FacilityDto, Facility>().ReverseMap();

        // KeyPoint <-> KeyPointDto
        CreateMap<KeyPointDto, KeyPoint>()
            .ForMember(dest => dest.Location,
                opt => opt.MapFrom(src => new GeoCoordinate(src.Latitude, src.Longitude)));

        // KeyPointDto <-> KeyPoint
        CreateMap<KeyPoint, KeyPointDto>()
            .ForMember(dest => dest.Latitude,
                opt => opt.MapFrom(src => src.Location.Latitude))
            .ForMember(dest => dest.Longitude,
                opt => opt.MapFrom(src => src.Location.Longitude));

        // TourDto -> Tour (create/update existing data)
        CreateMap<TourDto, Tour>()
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => MapStatus(src.Status)))
            // KeyPoints and LengthInKilometers are not set by DTO
            // aggreggate root manages them internally
            .ForMember(dest => dest.KeyPoints, opt => opt.Ignore())
            .ForMember(dest => dest.LengthInKilometers, opt => opt.Ignore());

        // Tour -> TourDto (answer to client)
        CreateMap<Tour, TourDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
<<<<<<< HEAD
        CreateMap<ShoppingCart, ShoppingCartDto>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
               .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
               .ReverseMap()
               .ConstructUsing(dto => new ShoppingCart(dto.UserId));
        CreateMap<OrderItem, OrderItemDto>().ReverseMap()
                .ConstructUsing(dto => new OrderItem(dto.TourId, dto.Price));
=======


        //CreateMap<TouristPreferencesDto, TouristPreferences>().ReverseMap();
        //ovu liniju iznad sam zakomentarisao i umesto toga sam napravio 2 odvojene funkcije koje mapiraju dto u domensku klasu i obrnuto
        CreateMap<TouristPreferences, Explorer.Tours.API.Dtos.TouristPreferencesDto>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(s => s.Difficulty.ToString()));


        CreateMap<Explorer.Tours.API.Dtos.TouristPreferencesDto, TouristPreferences>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(src => Enum.Parse<DifficultyLevel>(src.Difficulty)))
                .ForMember(d => d.TransportTypePreferences, o => o.Ignore());

        // Mapping za UpdateTouristPreferencesDto (bez PersonId)
        CreateMap<Explorer.Tours.API.Dtos.UpdateTouristPreferencesDto, TouristPreferences>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(src => Enum.Parse<DifficultyLevel>(src.Difficulty)))
                .ForMember(d => d.TransportTypePreferences, o => o.Ignore())
                .ForMember(d => d.PersonId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore());
                //.ForMember(d => d.Person, o => o.Ignore());

        //takodje sam to slicno uradio za TransportType
        CreateMap<TransportTypePreferences, Explorer.Tours.API.Dtos.TransportTypePreferenceDto>()
                .ForMember(d => d.Transport, o => o.MapFrom(s => s.Transport.ToString()))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.Rating));


        CreateMap<Explorer.Tours.API.Dtos.TransportTypePreferenceDto, TransportTypePreferences>()
                .ForMember(d => d.Transport, o => o.MapFrom(src => Enum.Parse<TransportType>(src.Transport)))
                .ForMember(d => d.Rating, o => o.MapFrom(src => src.Rating))
                .ForMember(d => d.PreferenceId, o => o.Ignore());



        //mapper za preference
        CreateMap<PreferenceTags, PreferenceTagsDto>().ReverseMap();
>>>>>>> origin/development
    }

    private static TourStatus MapStatus(string status)
    {
        if (Enum.TryParse<TourStatus>(status, out var parsed)) return parsed;
        return TourStatus.Draft;
    }
}
