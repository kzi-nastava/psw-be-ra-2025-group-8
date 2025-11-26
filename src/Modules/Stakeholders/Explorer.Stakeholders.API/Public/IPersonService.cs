using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IPersonService
{
    PersonDto GetByUserId(long userId);
    PersonDto UpdateProfile(long personId, UpdatePersonDto dto);
}
