using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class WalletService : IWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;

    public WalletService(IWalletRepository walletRepository, IMapper mapper)
    {
        _walletRepository = walletRepository;
        _mapper = mapper;
    }

    public WalletDto GetByUserId(long userId)
    {
        var wallet = _walletRepository.GetByUserId(userId);
        if (wallet == null)
            throw new NotFoundException($"Wallet not found for user {userId}.");

        return _mapper.Map<WalletDto>(wallet);
    }

    public WalletDto DepositCoins(long userId, int amount)
    {
        if (amount <= 0)
            throw new EntityValidationException("Amount must be positive.");

        var wallet = _walletRepository.GetByUserId(userId);
        if (wallet == null)
            throw new NotFoundException($"Wallet not found for user {userId}.");

        wallet.AddCoins(amount);
        var updatedWallet = _walletRepository.Update(wallet);

        return _mapper.Map<WalletDto>(updatedWallet);
    }
}
