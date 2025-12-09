using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPoint : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public string Secret { get; private set; }
    public int Order { get; internal set; }   // keypoint order within the tour
    public GeoCoordinate Location { get; private set; }

    private KeyPoint() { } // For EF

    public KeyPoint(string name, string description, string imageUrl, string secret, GeoCoordinate location, int order)
    {
        SetName(name);
        Description = description ?? string.Empty;
        ImageUrl = imageUrl ?? string.Empty;
        Secret = secret ?? string.Empty;
        Location = location ?? throw new ArgumentNullException(nameof(location));
        Order = order;
    }

    public void UpdateBasicInfo(string name, string description, string imageUrl, string secret)
    {
        SetName(name);
        Description = description ?? string.Empty;
        ImageUrl = imageUrl ?? string.Empty;
        Secret = secret ?? string.Empty;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Key point name is required.", nameof(name));

        Name = name.Trim();
    }
}
