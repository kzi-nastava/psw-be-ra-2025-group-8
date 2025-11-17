using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public string Content { get; set; }
        public DateTime TimestampCreated { get; set; }
        public DateTime? TimestampUpdated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
