namespace Explorer.Encounters.API.Dtos
{
    public class CheckEncounterRequestDto
    {
        public long PersonId { get; set; }
        public long EncounterId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class CheckEncounterResponseDto
    {
        public int ActiveCount { get; set; }
        public bool ThresholdReached { get; set; }
        public List<long> CompletedPersonIds { get; set; } = new();
    }
}
