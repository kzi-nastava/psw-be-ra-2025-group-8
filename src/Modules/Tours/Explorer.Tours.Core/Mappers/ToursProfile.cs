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
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString()));
        // KeyPoints and LengthInKilometers are being mapped automatically
    }

    private static TourStatus MapStatus(string status)
    {
        if (Enum.TryParse<TourStatus>(status, out var parsed)) return parsed;
        return TourStatus.Draft;
    }
}
