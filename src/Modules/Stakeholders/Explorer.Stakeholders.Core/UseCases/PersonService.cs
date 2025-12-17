using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class PersonService : IPersonService, IInternalPersonService
{
    private readonly ICrudRepository<Person> _personRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public PersonService(ICrudRepository<Person> personRepository, IUserRepository userRepository, IMapper mapper)
    {
        _personRepository = personRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public PersonDto GetByUserId(long userId)
    {
        var personId = _userRepository.GetPersonId(userId);
        var person = _personRepository.Get(personId);

        if (person == null)
            throw new NotFoundException("Person not found.");

        return _mapper.Map<PersonDto>(person);
    }

    public PersonDto UpdateProfile(long personId, UpdatePersonDto dto)
    {
        var person = _personRepository.Get(personId);

        if (person == null)
            throw new NotFoundException("Person not found.");

        person.UpdateProfile(
            dto.Name,
            dto.Surname,
            dto.Email,
            dto.ProfilePicture,
            dto.Bio,
            dto.Motto
        );

        var updatedPerson = _personRepository.Update(person);
        return _mapper.Map<PersonDto>(updatedPerson);
    }
}
