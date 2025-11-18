using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers;

public class StakeholderProfile : Profile
{
    public StakeholderProfile()
    {
        // Ako bude trebalo, dodaćeš i ostale mape (User, Person…), ali za ovaj zadatak
        // nam treba makar Message <-> MessageDto.

        CreateMap<Message, MessageDto>().ReverseMap();
    }
}
