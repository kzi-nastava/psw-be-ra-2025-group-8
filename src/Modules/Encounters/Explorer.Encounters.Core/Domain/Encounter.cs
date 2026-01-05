using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public enum EncouterStatus
    {
        Draft,
        Pending,
        Published,
        Archived
    }
    public enum EncouterType
    {
        SocialBased,
        LocationBased,
        MiscBased       //I dont know what this means
    }
    public class Encounter : AggregateRoot
    {
        public string Name { get; set; }
        public string Description { get; set; }
        // Sometimes maybe encouter doesnt have a specific coordinates, just a location name
        public string Location { get; set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public EncouterStatus Status { get; set; }
        public EncouterType Type { get; set; }
        public int XPReward { get; set; }
        public DateTime? PublishedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }

        // Creator (person) - optional
        public long? CreatorPersonId { get; set; }

        // Social encounter settings
        public int? SocialRequiredCount { get; private set; }
        public double? SocialRangeMeters { get; private set; }

        // Constructor for creating a new encounter (draft)
        public Encounter(string name, string description, string location, double? latitude, double? longitude, EncouterType type, int xpReward, int? socialRequiredCount = null, double? socialRangeMeters = null)
        {
            Name = name;
            Description = description;
            Location = location;
            Latitude = latitude;
            Longitude = longitude;
            Type = type;
            XPReward = xpReward;
            Status = EncouterStatus.Draft;
            PublishedAt = null;
            ArchivedAt = null;
            SocialRequiredCount = socialRequiredCount;
            SocialRangeMeters = socialRangeMeters;
        }

        public Encounter() { }

        //Publish encounter (allow from Draft or Pending)
        public void Publish()
        {
            if (Status == EncouterStatus.Published)
                throw new InvalidOperationException("Only draft or pending encounters can be published.");

            Status = EncouterStatus.Published;
            PublishedAt = DateTime.UtcNow;
        }
        //Archive encounter, only published encounters can be archived
        public void Archive()
        {
            if (Status != EncouterStatus.Published)
                throw new InvalidOperationException("Only published encounters can be archived.");
            Status = EncouterStatus.Archived;
            ArchivedAt = DateTime.UtcNow;
        }

        //Reactivate encounter, only archived encounters can be reactivated
        public void Reactivate()
        {
            if (Status != EncouterStatus.Archived)
                throw new InvalidOperationException("Only archived encounters can be reactivated.");
            Status = EncouterStatus.Published;
            ArchivedAt = null;
        }

        public void SetCoordinates(double? latitude, double? longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        // Allow creator to set social settings
        public void SetSocialSettings(int? requiredCount, double? rangeMeters)
        {
            if (requiredCount.HasValue && requiredCount <= 0)
                throw new ArgumentException("Social required count must be greater than 0.");

            if (rangeMeters.HasValue && rangeMeters <= 0)
                throw new ArgumentException("Social range meters must be greater than 0.");

            SocialRequiredCount = requiredCount;
            SocialRangeMeters = rangeMeters;
        }
    }
}
