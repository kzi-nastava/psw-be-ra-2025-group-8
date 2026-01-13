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
        private static readonly Dictionary<long, (decimal Price, int AuthorId)> TourData = new()
        {
            { -511, (50m, -11) },   // Beogradska avantura - Author -11
            { -522, (100m, -11) },  // Planinska tura - Author -11
            { -533, (70m, -11) },   // Dunavska ruta - Author -11
            { -544, (80m, -12) }    // Tura autora -12 (za unauthorized test)
        };

        public TourPriceDto? GetById(long id)
        {
            if (!TourData.TryGetValue(id, out var data)) return null;
            return new TourPriceDto 
            { 
                Id = id, 
                Price = data.Price,
                AuthorId = data.AuthorId
            };
        }
    }
}

