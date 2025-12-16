using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Follower : Entity
{
    public long UserId { get; private set; } // Ko prati
    public long FollowingUserId { get; private set; }  // Koga prati
    public DateTime FollowedAt { get; private set; }

    public Follower(long userId, long followingUserId)
    {
        if (userId == 0 || followingUserId == 0)
            throw new ArgumentException("Invalid user IDs.");

        if (userId == followingUserId)
            throw new ArgumentException("User cannot follow themselves.");

        UserId = userId;
        FollowingUserId = followingUserId;
        FollowedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private Follower() { }
}
