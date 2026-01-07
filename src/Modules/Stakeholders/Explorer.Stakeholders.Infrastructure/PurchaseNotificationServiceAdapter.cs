using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Infrastructure;

public class PurchaseNotificationServiceAdapter : IPurchaseNotificationService
{
    private readonly INotificationService _notificationService;

    public PurchaseNotificationServiceAdapter(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void NotifyTourPurchased(long touristUserId, long tourId)
    {
        _notificationService.Create(new NotificationDto
        {
            UserId = touristUserId,
            Type = (int)NotificationType.PurchaseSuccess,
            Title = "Purchase successful",
            Content = $"Tour #{tourId} has been added to your collection.",
            RelatedEntityId = tourId,
            RelatedEntityType = "Tour"
        });
    }

    public void NotifyToursPurchased(long touristUserId, IEnumerable<long> tourIds)
    {
        foreach (var id in tourIds.Distinct())
            NotifyTourPurchased(touristUserId, id);
    }
}
