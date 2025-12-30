using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class WalletService : IWalletService, IInternalWalletService
{
    private readonly IWalletRepository _walletRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;


    public WalletService(IWalletRepository walletRepository, IMapper mapper, INotificationService notificationService)
    {
        _walletRepository = walletRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }


    public WalletDto GetByUserId(long userId)
    {
        var wallet = _walletRepository.GetByUserId(userId);
        if (wallet == null)
            throw new NotFoundException($"Wallet not found for user {userId}.");

        return _mapper.Map<WalletDto>(wallet);
    }

    public WalletDto CreateWallet(long userId)
    {
        // Check if wallet already exists
        try
        {
            var existingWallet = _walletRepository.GetByUserId(userId);
            if (existingWallet != null)
                throw new InvalidOperationException($"Wallet already exists for user {userId}.");
        }
        catch (KeyNotFoundException)
        {
            // Wallet doesn't exist, we can create it
        }

        var wallet = new Wallet(userId);
        var createdWallet = _walletRepository.Create(wallet);

        return _mapper.Map<WalletDto>(createdWallet);
    }

    public WalletDto DepositCoins(long userId, int amount)
    {
        if (amount <= 0)
            throw new EntityValidationException("Amount must be positive.");

        Wallet wallet;
        try
        {
            wallet = _walletRepository.GetByUserId(userId);
        }
        catch (KeyNotFoundException)
        {
            // Wallet doesn't exist, create it automatically
            wallet = new Wallet(userId);
            wallet = _walletRepository.Create(wallet);
        }

        if (wallet == null)
        {
            // If still null after trying to get/create, create new one
            wallet = new Wallet(userId);
            wallet = _walletRepository.Create(wallet);
        }

        wallet.AddCoins(amount);
        var updatedWallet = _walletRepository.Update(wallet);

        // Notify user about the top-up
        _notificationService.Create(new NotificationDto
        {
            UserId = userId,
            Type = (int)NotificationType.WalletTopUp,
            Title = "Wallet top-up",
            Content = $"Your wallet was credited with {amount} AC. New balance: {updatedWallet.AdventureCoins} AC.",
            RelatedEntityId = updatedWallet.Id,
            RelatedEntityType = "Wallet"
        });

        return _mapper.Map<WalletDto>(updatedWallet);
    }

    public bool HasSufficientFunds(long userId, int requiredAmount)
    {
        var wallet = _walletRepository.GetByUserId(userId);
        if (wallet == null)
            return false;

        return wallet.AdventureCoins >= requiredAmount;
    }

    public void DeductCoins(long userId, int amount)
    {
        if (amount <= 0)
            throw new EntityValidationException("Amount must be positive.");

        var wallet = _walletRepository.GetByUserId(userId);
        if (wallet == null)
            throw new NotFoundException($"Wallet not found for user {userId}.");

        wallet.DeductCoins(amount);
        _walletRepository.Update(wallet);
    }
}
