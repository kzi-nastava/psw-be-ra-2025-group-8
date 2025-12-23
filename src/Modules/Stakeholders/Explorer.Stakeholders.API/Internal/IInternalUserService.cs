using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal;

public interface IInternalUserService
{
    UserDto? GetById(long id);
    Dictionary<long, UserDto> GetByIds(IEnumerable<long> userIds);
}
