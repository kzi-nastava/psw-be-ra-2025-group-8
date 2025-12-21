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
        Published,
        Archived
    }
    public enum EncouterType
    {
        SocialBased,
        LocationBased,
        MiscBased       //nzm sta ovo treba da predstavlja
    }
    public class Encounter : AggregateRoot
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public EncouterStatus Status { get; set; }
        public EncouterType Type { get; set; }
        public int XPReward { get; set; }
        public DateTime? PublishedAt { get; private set; }
        public DateTime? ArchivedAt { get; private set; }

        // Constructor for creating a new encounter (draft)
        public Encounter(string name, string description, string location, double latitude, double longitude, EncouterType type, int xpReward)
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
        }

        public Encounter() { }

        //Publish encounter
        public void Publish()
        {
            if (Status == EncouterStatus.Draft)
                throw new InvalidOperationException("Only non published encounters can be published.");
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




    }
}
