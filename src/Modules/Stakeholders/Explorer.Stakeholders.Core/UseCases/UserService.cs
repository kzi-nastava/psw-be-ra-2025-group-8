using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class UserService : IInternalUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public UserDto? GetById(long id)
    {
        var user = _userRepository.GetById(id);
        return user != null ? _mapper.Map<UserDto>(user) : null;
    }

    public Dictionary<long, UserDto> GetByIds(IEnumerable<long> userIds)
    {
        var result = new Dictionary<long, UserDto>();
        
        foreach (var userId in userIds.Distinct())
        {
            var user = _userRepository.GetById(userId);
            if (user != null)
            {
                result[userId] = _mapper.Map<UserDto>(user);
            }
        }
        
        return result;
    }
}
