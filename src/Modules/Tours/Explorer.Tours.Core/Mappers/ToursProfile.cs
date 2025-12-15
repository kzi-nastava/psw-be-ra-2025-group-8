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
            .ForMember(dest => dest.LengthInKilometers, opt => opt.Ignore())
            // RequiredEquipment is managed via separate methods
            .ForMember(dest => dest.RequiredEquipment, opt => opt.Ignore())
            .ForMember(dest => dest.TourTags, opt => opt.Ignore());

        // Tour -> TourDto (answer to client)
        CreateMap<Tour, TourDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.RequiredEquipment,
                opt => opt.MapFrom(src => src.RequiredEquipment))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.TourTags
                    .Select(tt => tt.Tags.Tag)
                    .ToList()));


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

        // TourEquipment -> TourEquipmentDto
        CreateMap<TourEquipment, TourEquipmentDto>()
            .ForMember(dest => dest.EquipmentId, opt => opt.MapFrom(src => src.EquipmentId))
            .ForMember(dest => dest.EquipmentName, opt => opt.MapFrom(src => src.Equipment.Name));

        // TourEquipmentDto -> TourEquipment (ignoring everything because new equipment is added via method)
        CreateMap<TourEquipmentDto, TourEquipment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TourId, opt => opt.Ignore())
            .ForMember(dest => dest.Tour, opt => opt.Ignore())
            .ForMember(dest => dest.EquipmentId, opt => opt.Ignore())
            .ForMember(dest => dest.Equipment, opt => opt.Ignore());
        //mapper za shopping cart i order item
        CreateMap<ShoppingCart, ShoppingCartDto>()
               .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
        CreateMap<OrderItem, OrderItemDto>().ReverseMap()
                .ConstructUsing(dto => new OrderItem(dto.TourId));
    }

    private static TourStatus MapStatus(string status)
    {
        if (Enum.TryParse<TourStatus>(status, out var parsed)) return parsed;
        return TourStatus.Draft;
    }
}
