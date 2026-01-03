using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Explorer.Tours.Core.Domain;

public class BundleTour
{
    public long BundleId { get; set; }
    public Bundle Bundle { get; set; }

    public long TourId { get; set; }
    public Tour Tour { get; set; }
}

