using System.Collections.Generic;
using System.Linq;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.ReposotoryInterfaces;

namespace Explorer.Encounters.Infrastructure.Database.Repositories;

public class ChallengeRepository : IChallengeRepository
{
    private readonly EncountersContext _context;
    public ChallengeRepository(EncountersContext context)
    {
        _context = context;
    }

    public Challenge Create(Challenge c)
    {
        _context.Challenges.Add(c);
        _context.SaveChanges();
        return c;
    }

    public Challenge Update(Challenge c)
    {
        _context.Challenges.Update(c);
        _context.SaveChanges();
        return c;
    }

    public Challenge Get(long id) => _context.Challenges.FirstOrDefault(x => x.Id == id);

    public IEnumerable<Challenge> GetPending() => _context.Challenges.Where(x => x.Status == ChallengeStatus.Pending).ToList();
}
