using System;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Meetup
    {
     
        public int Id { get; set; }

  
        public string Name { get; set; }

       
        public string Description { get; set; }

      
        public DateTime ScheduledAt { get; set; }

    
        public double Latitude { get; set; }
        public double Longitude { get; set; }

       
        public int CreatorId { get; set; }
        public User Creator { get; set; }

       
        public Meetup() { }

      
        public Meetup(
            string name,
            string description,
            DateTime scheduledAt,
            double latitude,
            double longitude,
            int creatorId)
        {
            Name = name;
            Description = description;
            ScheduledAt = scheduledAt;
            Latitude = latitude;
            Longitude = longitude;
            CreatorId = creatorId;
        }
    }
}
