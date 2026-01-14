using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Infrastructure.Database.Repositories
{
    public class BundlePurchaseRecordRepository : IBundlePurchaseRecordRepository
    {
        private readonly PaymentsContext _context;

        public BundlePurchaseRecordRepository(PaymentsContext context)
        {
            _context = context;
        }

        public void Add(BundlePurchaseRecord record)
        {
            _context.BundlePurchaseRecords.Add(record);
            _context.SaveChanges();
        }

        public List<BundlePurchaseRecord> GetByTouristId(long touristId)
        {
            return _context.BundlePurchaseRecords
                .Where(r => r.TouristId == touristId)
                .OrderByDescending(r => r.PurchaseDate)
                .ToList();
        }
    }
}

