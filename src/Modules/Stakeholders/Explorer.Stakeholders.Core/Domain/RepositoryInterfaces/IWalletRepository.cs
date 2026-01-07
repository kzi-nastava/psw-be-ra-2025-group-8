using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IWalletRepository
{
    Wallet? GetByUserId(long userId);
    Wallet Create(Wallet wallet);
    Wallet Update(Wallet wallet);
}
