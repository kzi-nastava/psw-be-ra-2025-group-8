using System;

namespace Explorer.Encounters.Core.Domain
{
    public class Challenge
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long CreatorPersonId { get; set; }
        public ChallengeStatus Status { get; set; } = ChallengeStatus.Pending;
        public int XPReward { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
    }

    public enum ChallengeStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
