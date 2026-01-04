using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;

namespace Explorer.Tours.Tests.TestHelpers;

public class MockInternalPersonService : IInternalPersonService
{
    public PersonDto GetByUserId(long userId)
    {
        return new PersonDto
        {
            Id = userId,
            UserId = userId,
            Name = "Test",
            Surname = "User",
            Email = "test.user@example.com",
            Experience = 0,
            Level = 1
        };
    }

    public PersonDto UpdateProfile(long personId, UpdatePersonDto dto)
    {
        return new PersonDto
        {
            Id = personId,
            UserId = personId,
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            ProfilePicture = dto.ProfilePicture,
            Bio = dto.Bio,
            Motto = dto.Motto,
            Experience = 0,
            Level = 1
        };
    }

    public PersonDto AddExperience(long userId, int xp)
    {
        // Simple mock logic: increase level every 100 XP
        var gainedLevels = xp / 100;
        var remainingXp = xp % 100;
        return new PersonDto
        {
            Id = userId,
            UserId = userId,
            Name = "Test",
            Surname = "User",
            Email = "test.user@example.com",
            Experience = remainingXp,
            Level = 1 + gainedLevels
        };
    }

    public PersonDto AddExperienceByPersonId(long personId, int xp)
    {
        // Mirror AddExperience behaviour for tests
        var gainedLevels = xp / 100;
        var remainingXp = xp % 100;
        return new PersonDto
        {
            Id = personId,
            UserId = personId,
            Name = "Test",
            Surname = "User",
            Email = "test.user@example.com",
            Experience = remainingXp,
            Level = 1 + gainedLevels
        };
    }
}
