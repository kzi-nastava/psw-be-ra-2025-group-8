using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly ICrudRepository<Person> _personRepository;

    public AuthenticationService(IUserRepository userRepository, ICrudRepository<Person> personRepository, ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _personRepository = personRepository;
    }

    public AuthenticationTokensDto Login(CredentialsDto credentials)
    {
        var user = _userRepository.GetActiveByName(credentials.Username);
        if (user == null || credentials.Password != user.Password)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        long personId;
        try
        {
            personId = _userRepository.GetPersonId(user.Id);
        }
        catch (KeyNotFoundException)
        {
            personId = 0;
        }
        return _tokenGenerator.GenerateAccessToken(user, personId);
    }

    public AuthenticationTokensDto RegisterTourist(AccountRegistrationDto account)
    {
        if(_userRepository.Exists(account.Username))
            throw new EntityValidationException("Provided username already exists.");

        var user = _userRepository.Create(new User(account.Username, account.Password, UserRole.Tourist, true));
        var person = _personRepository.Create(new Person(user.Id, account.Name, account.Surname, account.Email));

        return _tokenGenerator.GenerateAccessToken(user, person.Id);
    }

    public AccountDto CreateAccountByAdmin(AdminCreateAccountDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Role))
            throw new EntityValidationException("Role must be provided.");

        if (!Enum.TryParse<UserRole>(dto.Role, ignoreCase: true, out var role))
            throw new EntityValidationException("Invalid role.");

        if (role == UserRole.Tourist)
            throw new EntityValidationException("Cannot create Tourist via admin. Use tourist registration.");

        if (_userRepository.Exists(dto.Username))
            throw new EntityValidationException("Provided username already exists.");

        var user = new User(dto.Username, dto.Password, role, true);
        var createdUser = _userRepository.Create(user);

        // create a Person row (so email/name are stored)
        var person = _personRepository.Create(new Person(createdUser.Id, dto.Name ?? "", dto.Surname ?? "", dto.Email ?? ""));

        // map to AccountDto (no password)
        return new AccountDto
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Email = person.Email,
            Role = createdUser.Role.ToString(),
            IsActive = createdUser.IsActive,
            Name = person.Name,
            Surname = person.Surname
        };
    }

    public IEnumerable<AccountOverviewDto> GetAllAccounts()
    {
        var users = _userRepository.GetAll(); // uses repository method
        var result = new List<AccountOverviewDto>();

        foreach (var u in users)
        {
            // attempt to fetch person for email if exists
            long personId;
            string email = "";
            try
            {
                personId = _userRepository.GetPersonId(u.Id);
                // if you have a person repo or direct db access use it; otherwise query ICrudRepository<Person>
                var person = _personRepository.Get(personId);
                if (person != null) email = person.Email;
            }
            catch
            {
                // ignore if person missing
            }

            result.Add(new AccountOverviewDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                Email = email
            });
        }

        return result;
    }

    public void SetAccountActiveState(long userId, bool isActive)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) throw new NotFoundException("User not found.");

        // optionally prevent admin from deactivating the last admin, or self-block
        user.IsActive = isActive;
        _userRepository.Update(user);
    }

    public AccountDto GetById(long id)
    {
        var user = _userRepository.GetById(id);
        if (user == null) return null;

        string email = "";
        string name = "";
        string surname = "";

        try
        {
            var personId = _userRepository.GetPersonId(id);
            var person = _personRepository.Get(personId);
            if (person != null)
            {
                email = person.Email;
                name = person.Name;
                surname = person.Surname;
            }
        }
        catch
        {
            // user without person record → still allowed
        }

        return new AccountDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            Name = name,
            Surname = surname
        };
    }

}