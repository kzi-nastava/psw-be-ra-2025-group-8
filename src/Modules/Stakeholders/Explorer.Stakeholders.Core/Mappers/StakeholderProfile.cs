using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        //CreateMap<TouristPreferencesDto, TouristPreferences>().ReverseMap();
        //ovu liniju iznad sam zakomentarisao i umesto toga sam napravio 2 odvojene funkcije koje mapiraju dto u domensku klasu i obrnuto
        CreateMap<TouristPreferences, Explorer.Stakeholders.API.Dtos.TouristPreferencesDto>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(s => s.Difficulty.ToString()))
                .ForMember(d => d.TransportPreferences, o => o.MapFrom(s => s.TransportTypePreferences));


        CreateMap<Explorer.Stakeholders.API.Dtos.TouristPreferencesDto, TouristPreferences>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(src => Enum.Parse<DifficultyLevel>(src.Difficulty)))
                .ForMember(d => d.TransportTypePreferences, o => o.Ignore());

        //takodje sam to slicno uradio za TransportType
        CreateMap<TransportTypePreferences, Explorer.Stakeholders.API.Dtos.TransportTypePreferenceDto>()
                .ForMember(d => d.Transport, o => o.MapFrom(s => s.Transport.ToString()))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.Rating));


        CreateMap<Explorer.Stakeholders.API.Dtos.TransportTypePreferenceDto, TransportTypePreferences>()
                .ForMember(d => d.Transport, o => o.MapFrom(src => Enum.Parse<TransportType>(src.Transport)))
                .ForMember(d => d.Rating, o => o.MapFrom(src => src.Rating))
                .ForMember(d => d.PreferenceId, o => o.Ignore());
    }
}