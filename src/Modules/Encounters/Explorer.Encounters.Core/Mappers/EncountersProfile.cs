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
            .ReverseMap();

        CreateMap<EncounterParticipationDto, EncounterParticipation>().ReverseMap();
    }
}