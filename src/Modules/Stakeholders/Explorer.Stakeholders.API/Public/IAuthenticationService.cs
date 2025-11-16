using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IAuthenticationService
{
    AuthenticationTokensDto Login(CredentialsDto credentials);
    AuthenticationTokensDto RegisterTourist(AccountRegistrationDto account);


    // Admin operations
    AccountDto CreateAccountByAdmin(AdminCreateAccountDto account);            // create admin/author
    IEnumerable<AccountOverviewDto> GetAccounts();                         // list all users (no password)
    void ChangeAccountActivation(long userId, bool isActive);                   // block/unblock
    AccountDto GetById(long id);

}