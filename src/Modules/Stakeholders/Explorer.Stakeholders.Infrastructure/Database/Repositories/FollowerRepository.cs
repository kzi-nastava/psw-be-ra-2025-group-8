using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class FollowerRepository : IFollowerRepository
{
    private readonly StakeholdersContext _context;

    public FollowerRepository(StakeholdersContext context)
    {
        _context = context;
    }

    public List<Follower> GetFollowersByUserId(long userId)
    {
       return _context.Followers
                .Where(f => f.FollowingUserId == userId)
                .ToList();
    }

    public List<Follower> GetFollowingByUserId(long userId)
    {
       return _context.Followers
                .Where(f => f.UserId == userId)
                .ToList();
    }

    public Follower GetByUserIds(long userId, long followingUserId)
    {
        return _context.Followers
                .FirstOrDefault(f => f.UserId == userId && f.FollowingUserId == followingUserId);
    }

    public Follower Create(Follower follower)
    {
        _context.Followers.Add(follower);
        _context.SaveChanges();
        return follower;
    }

    public void Delete(long id)
    {
        var follower = _context.Followers.Find(id);
        if (follower != null)
        {
            _context.Followers.Remove(follower);
            _context.SaveChanges();
        }
    }
}
