using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Wallet : Entity
{
    public long UserId { get; private set; }
    public int AdventureCoins { get; private set; }

    protected Wallet() { }

    public Wallet(long userId)
    {
        if (userId <= 0) throw new ArgumentException("Invalid UserId.");
        
        UserId = userId;
        AdventureCoins = 0;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        AdventureCoins += amount;
    }

    public void DeductCoins(int amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        if (AdventureCoins < amount) throw new InvalidOperationException("Insufficient Adventure Coins.");
        AdventureCoins -= amount;
    }
}
