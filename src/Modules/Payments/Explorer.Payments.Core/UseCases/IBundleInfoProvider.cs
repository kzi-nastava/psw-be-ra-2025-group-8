using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Payments.Core.UseCases
{
    public class BundleInfoDto
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public List<long> TourIds { get; set; } = new();
    }

    public interface IBundleInfoProvider
    {
        // vrati null ako bundle ne postoji ili nije published
        BundleInfoDto? GetPublishedById(long id);
    }
}
