using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public;

public interface IWalletService
{
    WalletDto GetByUserId(long userId);
    WalletDto DepositCoins(long userId, int amount);
    WalletDto CreateWallet(long userId);
}

