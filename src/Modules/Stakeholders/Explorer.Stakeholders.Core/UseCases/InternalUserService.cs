using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class InternalUserService : IInternalUserService
{
    private readonly IUserRepository _userRepository;

    public InternalUserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string GetUsernameById(long userId)
    {
        var user = _userRepository.GetById(userId);
        return user?.Username ?? $"User#{userId}";
    }
}
