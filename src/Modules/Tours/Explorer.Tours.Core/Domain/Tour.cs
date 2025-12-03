using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Tour : AggregateRoot
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public TourStatus Status { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }

        // Aggregate
        public List<KeyPoint> KeyPoints { get; private set; } = new();
        public double LengthInKilometers { get; private set; }

        // Constructor for creating a new tour (draft, price=0)
        public Tour(string name, string description, int difficulty, List<string> tags, int authorId)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Status = TourStatus.Draft;
            Price = 0;
            AuthorId = authorId;
            KeyPoints = new List<KeyPoint>();
            LengthInKilometers = 0;
        }

        // For rehydrating an existing tour and tests
        public Tour(long id, string name, string description, int difficulty, List<string> tags,
                    TourStatus status, decimal price, int authorId)
        { 
            Id = id;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Status = status;
            Price = price;
            AuthorId = authorId;
            KeyPoints = new List<KeyPoint>();
            LengthInKilometers = 0;
        }

        // EF Core constructor
        public Tour()
        {
            Tags = new List<string>();
            KeyPoints = new List<KeyPoint>();
        }

        // Adding a key point to the tour
        public KeyPoint AddKeyPoint(string name, string description, string imageUrl, string secret, GeoCoordinate location)
        {
            if (Status != TourStatus.Draft)
                throw new InvalidOperationException("Key points can only be added while the tour is in preparation.");

            if (location == null) throw new ArgumentNullException(nameof(location));

            var order = KeyPoints.Count + 1;

            var keyPoint = new KeyPoint(name, description, imageUrl, secret, location, order);
            KeyPoints.Add(keyPoint);

            RecalculateLength();

            return keyPoint;
        }

        // Removing a key point from the tour
        public void RemoveKeyPoint(long keyPointId)
        {
            if (Status != TourStatus.Draft)
                throw new InvalidOperationException("Key points can only be removed while the tour is in preparation.");

            var existing = KeyPoints.SingleOrDefault(kp => kp.Id == keyPointId)
                           ?? throw new InvalidOperationException("Key point not found on this tour.");

            KeyPoints.Remove(existing);

            int order = 1;
            foreach (var kp in KeyPoints.OrderBy(k => k.Order))
            {
                kp.Order = order++;
            }

            RecalculateLength();
        }

        // Publishing the tour
        public void Publish()
        {
            if (Status != TourStatus.Draft)
                throw new InvalidOperationException("Only tours in preparation can be published.");

            if (string.IsNullOrWhiteSpace(Name) ||
                string.IsNullOrWhiteSpace(Description) ||
                Difficulty <= 0 ||
                Tags == null || Tags.Count == 0)
            {
                throw new InvalidOperationException("Tour basics must be filled in before publishing.");
            }

            if (KeyPoints.Count < 2)
                throw new InvalidOperationException("Tour must have at least two key points before publishing.");

            Status = TourStatus.Published;
        }

        public void Archive()
        {
            Status = TourStatus.Archived;
        }

        // Calculate total length based on key points
        private void RecalculateLength()
        {
            if (KeyPoints == null || KeyPoints.Count < 2)
            {
                LengthInKilometers = 0;
                return;
            }

            var ordered = KeyPoints.OrderBy(k => k.Order).ToList();
            double total = 0;

            for (int i = 1; i < ordered.Count; i++)
            {
                total += ordered[i - 1].Location.DistanceTo(ordered[i].Location);
            }

            LengthInKilometers = Math.Round(total, 3);
        }
    }


    public enum TourStatus
    {
        Draft,
        Published,
        Archived
    }
}