using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Blog.Core.Domain;

// Vote is an entity in BlogPost aggregate
public class Vote : Entity
{
    public long PersonId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public VoteType Type { get; private set; }

    protected Vote() { }

    public Vote(long personId, VoteType type)
    {
        if (personId == 0) throw new ArgumentException("Invalid PersonId");
        
        PersonId = personId;
        Type = type;
        CreatedAt = DateTime.UtcNow;
    }

    public void ChangeVote(VoteType newType)
    {
        Type = newType;
    }
}

public enum VoteType
{
    Upvote = 1,
    Downvote = -1
}

