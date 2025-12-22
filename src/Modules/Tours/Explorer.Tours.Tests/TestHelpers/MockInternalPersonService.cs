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
            Email = "test.user@example.com"
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
            Motto = dto.Motto
        };
    }

    public Dictionary<long, PersonDto> GetByUserIds(IEnumerable<long> userIds)
    {
        var result = new Dictionary<long, PersonDto>();
        
        foreach (var userId in userIds)
        {
            result[userId] = new PersonDto
            {
                Id = userId,
                UserId = userId,
                Name = "Test",
                Surname = "User",
                Email = $"test.user{userId}@example.com"
            };
        }
        
        return result;
    }
}
