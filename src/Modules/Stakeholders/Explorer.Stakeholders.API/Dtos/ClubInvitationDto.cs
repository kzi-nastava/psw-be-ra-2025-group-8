using System;

namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubInvitationDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public string ClubName { get; set; }
        public long TouristId { get; set; }
        public string TouristName { get; set; }
        public string TouristSurname { get; set; }
        public string TouristUsername { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
