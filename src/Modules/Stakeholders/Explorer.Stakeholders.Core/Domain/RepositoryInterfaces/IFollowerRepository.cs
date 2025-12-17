using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IFollowerRepository
{
    List<Follower> GetFollowersByUserId(long userId);
    List<Follower> GetFollowingByUserId(long userId);
    Follower GetByUserIds(long userId, long followingUserId);
    Follower Create(Follower follower);
    void Delete(long id);
}
