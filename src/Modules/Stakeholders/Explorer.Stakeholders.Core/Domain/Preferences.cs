using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum DifficultyLevel
    {
        Beginner,
        Intermediate,
        Professional
    }
    public enum TransportType
    {
        Walk,
        Bicycle,
        Car,
        Boat
    }

    public class Preferences : Entity
    {
        public long PersonId { get; set; }
        public Person Person { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public Dictionary<TransportType, int> TransportRatings { get; set; }
        public List<string> Tags { get; set; }

        public Preferences(long personId, DifficultyLevel difficulty, Dictionary<TransportType, int> transportRatings, List<string> tags)
        {
            if (transportRatings == null)
                throw new ArgumentException("Transport ratings cannot be null.");

            foreach (var rating in transportRatings)
            {
                if (rating.Value < 0 || rating.Value > 3)
                    throw new ArgumentException($"Rating for {rating.Key} must be between 0 and 3.");
            }

            PersonId = personId;
            Difficulty = difficulty;
            TransportRatings = transportRatings;
            Tags = tags ?? new List<string>();
        }
    }
}

