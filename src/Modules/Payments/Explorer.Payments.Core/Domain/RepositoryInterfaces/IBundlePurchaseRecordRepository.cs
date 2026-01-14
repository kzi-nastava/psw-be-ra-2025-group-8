using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.Domain;

namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface IBundlePurchaseRecordRepository
    {
        void Add(BundlePurchaseRecord record);
        List<BundlePurchaseRecord> GetByTouristId(long touristId);
    }
}

