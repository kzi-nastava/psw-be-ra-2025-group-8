using System;

namespace Explorer.Encounters.API.Dtos
{
    public class EncounterWithStatusDto
    {
        public EncounterDto Encounter { get; set; }
        public string ParticipationStatus { get; set; } // "NotStarted", "Active", "Completed", "Abandoned"
        public DateTime? ActivatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? XPAwarded { get; set; }
    }
}