using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using static Explorer.Tours.Core.Domain.TourExecution;

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

        CreateMap<TourExecutionDto, TourExecution>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapTourExecutionStatus(src.Status)));

        CreateMap<TourExecution, TourExecutionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }

    private static TourStatus MapStatus(string status)
    {
        if (Enum.TryParse<TourStatus>(status, out var parsed)) return parsed;
        return TourStatus.Draft;
    }

    private static TourExecutionStatus MapTourExecutionStatus(string status)
    {
        if (Enum.TryParse<TourExecutionStatus>(status, out var parsed)) return parsed;
        return TourExecutionStatus.Completed;
    }
}