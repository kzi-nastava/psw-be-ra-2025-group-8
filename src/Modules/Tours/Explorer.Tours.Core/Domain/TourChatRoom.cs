using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourChatRoom : Entity
    {
        public long TourId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<TourChatMember> _members = new();
        public IReadOnlyCollection<TourChatMember> Members => _members.AsReadOnly();

        private readonly List<TourChatMessage> _messages = new();
        public IReadOnlyCollection<TourChatMessage> Messages => _messages.AsReadOnly();

        protected TourChatRoom() { }

        public TourChatRoom(long tourId, string tourName)
        {
            if (tourId <= 0)
                throw new ArgumentException("Tour ID must be positive", nameof(tourId));
            
            if (string.IsNullOrWhiteSpace(tourName))
                throw new ArgumentException("Tour name cannot be empty", nameof(tourName));

            TourId = tourId;
            Name = $"Tour Chat: {tourName}";
            Description = $"Chat room for tourists on tour #{tourId}";
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void AddMember(long userId)
        {
            if (_members.Any(m => m.UserId == userId && m.IsActive))
                return; // Already a member

            var member = new TourChatMember(Id, userId);
            _members.Add(member);
        }

        public void RemoveMember(long userId)
        {
            var member = _members.FirstOrDefault(m => m.UserId == userId && m.IsActive);
            if (member != null)
            {
                member.Leave();
            }
        }

        public void AddMessage(long senderId, string content)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot send messages to inactive chat room");

            if (!_members.Any(m => m.UserId == senderId && m.IsActive))
                throw new InvalidOperationException("Only members can send messages");

            var message = new TourChatMessage(Id, senderId, content);
            _messages.Add(message);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public bool IsMember(long userId)
        {
            return _members.Any(m => m.UserId == userId && m.IsActive);
        }
    }
}
