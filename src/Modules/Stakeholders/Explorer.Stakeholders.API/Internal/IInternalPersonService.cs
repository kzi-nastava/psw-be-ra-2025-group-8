using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal;

public interface IInternalPersonService
{
    PersonDto GetByUserId(long userId);
    PersonDto GetByPersonId(long personId);
    PersonDto UpdateProfile(long personId, UpdatePersonDto dto);
    PersonDto AddExperience(long userId, int xp);
    PersonDto AddExperienceByPersonId(long personId, int xp);
}
