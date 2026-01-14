using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class Bundle : AggregateRoot
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int AuthorId { get; set; }
    public BundleStatus Status { get; private set; }

    public DateTime? PublishedAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }

    public List<BundleTour> BundleTours { get; private set; } = new();

    public Bundle(string name, decimal price, int authorId, IEnumerable<long> tourIds)
    {
        Name = name;
        Price = price;
        AuthorId = authorId;
        Status = BundleStatus.Draft;

        SetTours(tourIds);
    }

    // EF
    public Bundle() { }

    public void SetTours(IEnumerable<long> tourIds)
    {
        var ids = tourIds?.Distinct().ToList() ?? new List<long>();
        if (ids.Count < 2) throw new ArgumentException("Bundle must contain at least 2 tours.");

        BundleTours = ids.Select(id => new BundleTour { TourId = id }).ToList();
    }

    public void Publish(int publishedToursCount)
    {
        if (Status != BundleStatus.Draft)
            throw new InvalidOperationException("Only draft bundle can be published.");

        if (publishedToursCount < 2)
            throw new InvalidOperationException("Bundle can be published only if it contains at least 2 published tours.");

        Status = BundleStatus.Published;
        PublishedAt = DateTime.UtcNow;
        ArchivedAt = null;
    }

    public void Archive()
    {
        if (Status != BundleStatus.Published)
            throw new InvalidOperationException("Only published bundle can be archived.");

        Status = BundleStatus.Archived;
        ArchivedAt = DateTime.UtcNow;
    }
}

public enum BundleStatus
{
    Draft,
    Published,
    Archived
}

