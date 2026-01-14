using System;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourChatMember : Entity
    {
        public long TourChatRoomId { get; private set; }
        public long UserId { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public DateTime? LeftAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastReadAt { get; private set; }

        protected TourChatMember() { }

        public TourChatMember(long tourChatRoomId, long userId)
        {
            TourChatRoomId = tourChatRoomId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Leave()
        {
            IsActive = false;
            LeftAt = DateTime.UtcNow;
        }

        public void Rejoin()
        {
            if (!IsActive)
            {
                IsActive = true;
                JoinedAt = DateTime.UtcNow;
                LeftAt = null;
            }
        }

        public void MarkAsRead()
        {
            LastReadAt = DateTime.UtcNow;
        }
    }
}
