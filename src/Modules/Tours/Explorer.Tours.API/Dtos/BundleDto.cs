using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Explorer.Tours.API.Dtos;

public class BundleDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int AuthorId { get; set; }
    public string Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public List<TourInBundleDto> Tours { get; set; } = new();
}

public class TourInBundleDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public decimal Price { get; set; }
}

public class CreateBundleDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<long> TourIds { get; set; } = new();
}

public class UpdateBundleDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public List<long> TourIds { get; set; } = new();
}

