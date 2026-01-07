using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface IBundlePurchaseService
    {
        BundlePurchaseRecordDto PurchasePublishedBundle(long touristUserId, long bundleId);
    }
}

