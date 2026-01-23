using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Internal;

public interface IInternalWalletService
{
    bool HasSufficientFunds(long userId, int requiredAmount);
    void DeductCoins(long userId, int amount);
    WalletDto DepositCoins(long userId, int amount);
}
