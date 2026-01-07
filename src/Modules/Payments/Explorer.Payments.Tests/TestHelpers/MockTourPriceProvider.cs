using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.UseCases;

namespace Explorer.Payments.Tests.TestHelpers
{
    public class MockTourPriceProvider : ITourPriceProvider
    {
        private static readonly Dictionary<long, decimal> Prices = new()
        {
            { -511, 50m },
            { -522, 100m },
            { -533, 70m }
            // Add other seeded IDs if needed
        };

        public TourPriceDto? GetById(long id)
        {
            if (!Prices.TryGetValue(id, out var p)) return null;
            return new TourPriceDto { Id = id, Price = p };
        }
    }
}
