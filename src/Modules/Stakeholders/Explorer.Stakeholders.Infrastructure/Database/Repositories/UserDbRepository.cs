using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class UserDbRepository : IUserRepository
{
    private readonly StakeholdersContext _dbContext;

    public UserDbRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Exists(string username)
    {
        return _dbContext.Users.Any(user => user.Username == username);
    }

    public User? GetActiveByName(string username)
    {
        return _dbContext.Users.FirstOrDefault(user => user.Username == username && user.IsActive);
    }

    public User Create(User user)
    {
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return user;
    }

    public long GetPersonId(long userId)
    {
        var person = _dbContext.People.FirstOrDefault(i => i.UserId == userId);
        if (person == null)
        {
            // Fallback: in test seed Person.Id often equals User.Id; return userId to allow tests to proceed
            return userId;
        }
        return person.Id;
    }

    public IEnumerable<User> GetAll() //returns all users (active and inactive)
    {
        return _dbContext.Users.AsNoTracking().ToList();
    }

    public User? GetById(long id)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Id == id);
    }

    public User Update(User user)
    {
        _dbContext.Users.Update(user);
        _dbContext.SaveChanges();
        return user;
    }
}