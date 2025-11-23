using AutoMapper;
using Explorer.Clubs.API.Dtos;
using Explorer.Clubs.Core.Domain;

namespace Explorer.Clubs.Core.Mappers
{
    public class ClubsProfile : Profile
    {
        public ClubsProfile()
        {
            CreateMap<Club, ClubDto>();
            CreateMap<ClubDto, Club>()
                .ConstructUsing(dto => new Club(dto.OwnerId, dto.Name, dto.Description, dto.ImageUrls));
            CreateMap<CreateClubDto, Club>()
                .ConstructUsing(dto => new Club(0, dto.Name, dto.Description, dto.ImageUrls));
        }
    }
}
