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
        private static readonly Dictionary<long, (decimal Price, int AuthorId)> Tours = new()
        {
            { -511, (50m, -11) },
            { -522, (100m, -11) },
            { -533, (70m, -11) }
            ,{ -544, (80m, -12) }
            // Add other seeded IDs if needed
        };

        public TourPriceDto? GetById(long id)
        {
            if (!Tours.TryGetValue(id, out var t)) return null;
            return new TourPriceDto { Id = id, Price = t.Price, AuthorId = t.AuthorId };
        }
    }
}
