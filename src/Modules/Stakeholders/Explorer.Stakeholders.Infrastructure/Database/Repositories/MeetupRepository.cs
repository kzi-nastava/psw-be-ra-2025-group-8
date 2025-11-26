using System.Collections.Generic;
using System.Linq;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class MeetupRepository : IMeetupRepository
{
    private readonly StakeholdersContext _context;

    public MeetupRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public Meetup Create(Meetup meetup)
    {
        _context.Meetups.Add(meetup);
        _context.SaveChanges();
        return meetup;
    }

    public Meetup Update(Meetup meetup)
    {
        _context.Meetups.Update(meetup);
        _context.SaveChanges();
        return meetup;
    }

    public void Delete(int id)
    {
        var entity = _context.Meetups.FirstOrDefault(x => x.Id == id);
        if (entity == null) return;
        _context.Meetups.Remove(entity);
        _context.SaveChanges();
    }

    public Meetup Get(int id)
    {
        return _context.Meetups.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<Meetup> GetByCreator(long creatorId)
    {
        return _context.Meetups.Where(x => x.CreatorId == creatorId).ToList();
    }
}
