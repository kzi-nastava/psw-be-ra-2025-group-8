using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Payments.Core.UseCases;

namespace Explorer.Payments.Tests.TestHelpers;

/// <summary>
/// Test double for bundle lookups.
///
/// Payments module depends on <see cref="IBundleInfoProvider"/> to fetch *published* bundle info.
/// In integration tests we keep that dependency deterministic and independent from Tours module.
/// </summary>
public class MockBundleInfoProvider : IBundleInfoProvider
{
    // Keep IDs negative to avoid collisions with DB-generated IDs.
    public const long ExistingBundleId = -9001;
    public const decimal ExistingBundlePrice = 120.50m;
    public static readonly List<long> ExistingBundleTourIds = new() { -511, -522 };

    public const long ExpensiveBundleId = -9002;
    public const decimal ExpensiveBundlePrice = 9999m;
    public static readonly List<long> ExpensiveBundleTourIds = new() { -511, -522, -533 };

    public BundleInfoDto? GetPublishedById(long id)
    {
        if (id == ExistingBundleId)
        {
            return new BundleInfoDto
            {
                Id = ExistingBundleId,
                Price = ExistingBundlePrice,
                TourIds = ExistingBundleTourIds.ToList()
            };
        }

        if (id == ExpensiveBundleId)
        {
            return new BundleInfoDto
            {
                Id = ExpensiveBundleId,
                Price = ExpensiveBundlePrice,
                TourIds = ExpensiveBundleTourIds.ToList()
            };
        }

        return null;
    }
}

