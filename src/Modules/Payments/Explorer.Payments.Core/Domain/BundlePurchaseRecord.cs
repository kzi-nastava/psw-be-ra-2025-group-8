using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Payments.Core.Domain
{
    public class BundlePurchaseRecord : Entity
    {
        public long TouristId { get; private set; }
        public long BundleId { get; private set; }
        public decimal Price { get; private set; }
        public int AdventureCoinsSpent { get; private set; }
        public DateTime PurchaseDate { get; private set; }

        private BundlePurchaseRecord() { }

        public BundlePurchaseRecord(long touristId, long bundleId, decimal price, int adventureCoinsSpent)
        {
            TouristId = touristId;
            BundleId = bundleId;
            Price = price;
            AdventureCoinsSpent = adventureCoinsSpent;
            PurchaseDate = DateTime.UtcNow;
        }
    }
}
