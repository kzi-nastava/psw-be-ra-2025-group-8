using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class StakeholdersProfile : Profile
    {
        public StakeholdersProfile()
        {
            CreateMap<Message, MessageDto>().ReverseMap();
        }
    }
}
