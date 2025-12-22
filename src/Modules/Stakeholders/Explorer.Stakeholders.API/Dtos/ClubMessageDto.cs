using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMessageDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public long AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorSurname { get; set; }
        public string AuthorUsername { get; set; }
        public string Content { get; set; }
        public DateTime TimestampCreated { get; set; }
        public DateTime? TimestampUpdated { get; set; }
    }
}
