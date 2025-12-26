using Explorer.BuildingBlocks.Core.Domain;
using System;

namespace Explorer.Encounters.Core.Domain
{
    public enum ParticipationStatus
    {
        Active,
        Completed,
        Abandoned
    }

    public class EncounterParticipation : Entity
    {
        public long PersonId { get; private set; }
        public long EncounterId { get; private set; }
        public ParticipationStatus Status { get; private set; }
        public DateTime ActivatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public int? XPAwarded { get; private set; }

        // Constructor for creating new participation
        public EncounterParticipation(long personId, long encounterId)
        {
            PersonId = personId;
            EncounterId = encounterId;
            Status = ParticipationStatus.Active;
            ActivatedAt = DateTime.UtcNow;
            CompletedAt = null;
            XPAwarded = null;
        }

        // EF Core constructor
        public EncounterParticipation() { }

        // Complete the encounter and award XP
        public void Complete(int xpReward)
        {
            if (Status == ParticipationStatus.Completed)
                throw new InvalidOperationException("Encounter is already completed.");

            Status = ParticipationStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            XPAwarded = xpReward;
        }

        // Abandon the encounter
        public void Abandon()
        {
            if (Status == ParticipationStatus.Completed)
                throw new InvalidOperationException("Completed encounters cannot be abandoned.");

            Status = ParticipationStatus.Abandoned;
        }

        // Reactivate an abandoned encounter
        public void Reactivate()
        {
            if (Status != ParticipationStatus.Abandoned)
                throw new InvalidOperationException("Only abandoned encounters can be reactivated.");

            Status = ParticipationStatus.Active;
        }
    }
}