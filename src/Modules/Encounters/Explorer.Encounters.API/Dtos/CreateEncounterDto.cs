namespace Explorer.Encounters.API.Dtos
{
    public class CreateEncounterDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Type { get; set; }
        public int XPReward { get; set; }
        public int? SocialRequiredCount { get; set; }
        public double? SocialRangeMeters { get; set; }
    }
}
