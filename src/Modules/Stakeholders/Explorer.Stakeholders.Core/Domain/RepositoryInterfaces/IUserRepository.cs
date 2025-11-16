namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IUserRepository
{
    bool Exists(string username);
    User? GetActiveByName(string username);
    User Create(User user);
    long GetPersonId(long userId);
    IEnumerable<User> GetAll();           // get all users (active+inactive)
    User? GetById(long id);
    User Update(User user);              
}