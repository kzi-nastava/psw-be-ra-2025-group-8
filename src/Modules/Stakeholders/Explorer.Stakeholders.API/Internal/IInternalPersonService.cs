using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal;

public interface IInternalPersonService
{
    PersonDto GetByUserId(long userId);
    PersonDto UpdateProfile(long personId, UpdatePersonDto dto);
}
