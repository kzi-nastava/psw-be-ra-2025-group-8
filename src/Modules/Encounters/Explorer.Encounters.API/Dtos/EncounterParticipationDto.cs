using System;

namespace Explorer.Encounters.API.Dtos
{
    public class EncounterParticipationDto
    {
        public long PersonId { get; set; }
        public long EncounterId { get; set; }
        public string Status { get; set; }
        public DateTime ActivatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? XPAwarded { get; set; }
    }

    public class ActivateEncounterDto
    {
        public long PersonId { get; set; }
        public long EncounterId { get; set; }
    }

    public class CompleteEncounterDto
    {
        public long PersonId { get; set; }
        public long EncounterId { get; set; }
    }
}