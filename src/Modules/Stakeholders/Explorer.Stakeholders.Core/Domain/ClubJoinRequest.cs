using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubJoinRequest : Entity
    {
        public long ClubId { get; private set; }
        public long TouristId { get; private set; }
        public ClubJoinRequestStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected ClubJoinRequest() { }

        public ClubJoinRequest(long clubId, long touristId)
        {
            if (clubId == 0) throw new ArgumentException("Invalid ClubId");
            if (touristId == 0) throw new ArgumentException("Invalid TouristId");

            ClubId = clubId;
            TouristId = touristId;
            Status = ClubJoinRequestStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Accept()
        {
            if (Status != ClubJoinRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be accepted.");
            
            Status = ClubJoinRequestStatus.Accepted;
        }

        public void Reject()
        {
            if (Status != ClubJoinRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be rejected.");
            
            Status = ClubJoinRequestStatus.Rejected;
        }

        public void Cancel()
        {
            if (Status != ClubJoinRequestStatus.Pending)
                throw new InvalidOperationException("Only pending requests can be cancelled.");
            
            Status = ClubJoinRequestStatus.Cancelled;
        }
    }

    public enum ClubJoinRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Cancelled = 3
    }
}
