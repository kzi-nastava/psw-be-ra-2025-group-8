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
                .ForMember(d => d.Difficulty, o => o.MapFrom(s => s.Difficulty.ToString()));


        CreateMap<Explorer.Stakeholders.API.Dtos.TouristPreferencesDto, TouristPreferences>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(src => Enum.Parse<DifficultyLevel>(src.Difficulty)))
                .ForMember(d => d.TransportTypePreferences, o => o.Ignore());

        // Mapping za UpdateTouristPreferencesDto (bez PersonId)
        CreateMap<Explorer.Stakeholders.API.Dtos.UpdateTouristPreferencesDto, TouristPreferences>()
                .ForMember(d => d.Difficulty, o => o.MapFrom(src => Enum.Parse<DifficultyLevel>(src.Difficulty)))
                .ForMember(d => d.TransportTypePreferences, o => o.Ignore())
                .ForMember(d => d.PersonId, o => o.Ignore())
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.Person, o => o.Ignore());

        //takodje sam to slicno uradio za TransportType
        CreateMap<TransportTypePreferences, Explorer.Stakeholders.API.Dtos.TransportTypePreferenceDto>()
                .ForMember(d => d.Transport, o => o.MapFrom(s => s.Transport.ToString()))
                .ForMember(d => d.Rating, o => o.MapFrom(s => s.Rating));


        CreateMap<Explorer.Stakeholders.API.Dtos.TransportTypePreferenceDto, TransportTypePreferences>()
                .ForMember(d => d.Transport, o => o.MapFrom(src => Enum.Parse<TransportType>(src.Transport)))
                .ForMember(d => d.Rating, o => o.MapFrom(src => src.Rating))
                .ForMember(d => d.PreferenceId, o => o.Ignore());



        //mapper za preference
        CreateMap<PreferenceTags, PreferenceTagsDto>().ReverseMap();

        CreateMap<RatingDto, Rating>().ReverseMap();

        CreateMap<RatingNoIdDto, Rating>();

        CreateMap<Person, PersonDto>().ReverseMap();

        CreateMap<Message, MessageDto>().ReverseMap();

        //mapper za klubove, nisam koristio ReverseMap, jer mi ne trebaju sva mapiranja za 2 dto-a i 1 entity
        CreateMap<Club, ClubDto>();
        CreateMap<ClubDto, Club>()
            .ConstructUsing(dto => new Club(dto.OwnerId, dto.Name, dto.Description, dto.ImageUrls));
        CreateMap<CreateClubDto, Club>()
            .ConstructUsing(dto => new Club(0, dto.Name, dto.Description, dto.ImageUrls));
    }
}
