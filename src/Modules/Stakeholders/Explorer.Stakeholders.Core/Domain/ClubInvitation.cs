using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubInvitation : Entity
    {
        public long ClubId { get; private set; }
        public long TouristId { get; private set; }
        public ClubInvitationStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected ClubInvitation() { }

        public ClubInvitation(long clubId, long touristId)
        {
            if (clubId == 0) throw new ArgumentException("Invalid ClubId");
            if (touristId == 0) throw new ArgumentException("Invalid TouristId");

            ClubId = clubId;
            TouristId = touristId;
            Status = ClubInvitationStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void Accept()
        {
            if (Status != ClubInvitationStatus.Pending)
                throw new InvalidOperationException("Only pending invitations can be accepted.");
            
            Status = ClubInvitationStatus.Accepted;
        }

        public void Reject()
        {
            if (Status != ClubInvitationStatus.Pending)
                throw new InvalidOperationException("Only pending invitations can be rejected.");
            
            Status = ClubInvitationStatus.Rejected;
        }

        public void Cancel()
        {
            if (Status != ClubInvitationStatus.Pending)
                throw new InvalidOperationException("Only pending invitations can be cancelled.");
            
            Status = ClubInvitationStatus.Cancelled;
        }
    }

    public enum ClubInvitationStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        Cancelled = 3
    }
}
