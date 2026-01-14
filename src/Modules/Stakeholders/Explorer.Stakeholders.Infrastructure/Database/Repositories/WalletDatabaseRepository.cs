using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories;

public class WalletDatabaseRepository : IWalletRepository
{
    private readonly StakeholdersContext _dbContext;

    public WalletDatabaseRepository(StakeholdersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Wallet? GetByUserId(long userId)
    {
        return _dbContext.Wallets.FirstOrDefault(w => w.UserId == userId);
    }

    public Wallet Create(Wallet wallet)
    {
        _dbContext.Wallets.Add(wallet);
        _dbContext.SaveChanges();
        return wallet;
    }

    public Wallet Update(Wallet wallet)
    {
        _dbContext.Wallets.Update(wallet);
        _dbContext.SaveChanges();
        return wallet;
    }
}
