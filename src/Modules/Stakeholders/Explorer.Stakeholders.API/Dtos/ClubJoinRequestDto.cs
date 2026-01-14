using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubJoinRequestDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public long TouristId { get; set; }
        public string? TouristUsername { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
