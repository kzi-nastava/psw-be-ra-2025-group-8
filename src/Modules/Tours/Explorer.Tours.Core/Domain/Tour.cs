using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Tour : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public TourStatus Status { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }

        // Constructor for creating a new tour (Draft, Price = 0)
        public Tour(string name, string description, int difficulty, List<string> tags, int authorId)
        {
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags ?? new List<string>();
            Status = TourStatus.Draft;
            Price = 0;
            AuthorId = authorId;
        }

        // Constructor for full initialization (for EF Core hydration or tests)
        public Tour(int id, string name, string description, int difficulty, List<string> tags, TourStatus status, decimal price, int authorId)
        {
            Id = id;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            Tags = tags;
            Status = status;
            Price = price;
            AuthorId = authorId;
        }

        public Tour() { }
    }

    public enum TourStatus
    {
        Draft,
        Published,
        Archived
    }
}