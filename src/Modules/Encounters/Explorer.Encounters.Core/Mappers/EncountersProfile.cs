using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Mappers;

public class EncountersProfile : Profile
{
    public EncountersProfile()
    {
        CreateMap<EncounterDto, Encounter>()
            .ForMember(dest => dest.SocialRequiredCount, opt => opt.MapFrom(src => src.SocialRequiredCount))
            .ForMember(dest => dest.SocialRangeMeters, opt => opt.MapFrom(src => src.SocialRangeMeters))
            .ForMember(dest => dest.ImageLatitude, opt => opt.MapFrom(src => src.ImageLatitude))
            .ForMember(dest => dest.ImageLongitude, opt => opt.MapFrom(src => src.ImageLongitude))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ReverseMap();

        CreateMap<EncounterParticipation, EncounterParticipationDto>()
            .ForMember(dest => dest.StartTimeInRange, opt => opt.MapFrom(src => src.StartTimeInRange))
            .ReverseMap();
    }
}