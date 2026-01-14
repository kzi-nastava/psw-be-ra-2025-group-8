using System.Collections.Generic;

namespace Explorer.BuildingBlocks.Core.UseCases;

public interface IPurchaseNotificationService
{
    void NotifyTourPurchased(long touristUserId, long tourId);
    void NotifyToursPurchased(long touristUserId, IEnumerable<long> tourIds);
}
