using AutoMapper;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        CreateMap<RatingDto, Rating>().ReverseMap();

        CreateMap<RatingNoIdDto, Rating>();

        CreateMap<Person, PersonDto>().ReverseMap();

        CreateMap<Message, MessageDto>().ReverseMap();

        CreateMap<Notification, NotificationDto>().ReverseMap();

        //mapper za klubove, nisam koristio ReverseMap, jer mi ne trebaju sva mapiranja za 2 dto-a i 1 entity
        CreateMap<Club, ClubDto>();
        CreateMap<ClubDto, Club>()
            .ConstructUsing(dto => new Club(dto.OwnerId, dto.Name, dto.Description, dto.ImageUrls));
        CreateMap<CreateClubDto, Club>()
            .ConstructUsing(dto => new Club(0, dto.Name, dto.Description, dto.ImageUrls));
    }
}
