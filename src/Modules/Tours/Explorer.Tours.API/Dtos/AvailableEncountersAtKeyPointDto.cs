namespace Explorer.Tours.API.Dtos;

public class AvailableEncountersAtKeyPointDto
{
    public int KeyPointOrder { get; set; }
    public string KeyPointName { get; set; }
    public bool IsAtKeyPoint { get; set; }
    public double DistanceToKeyPointMeters { get; set; }
    public List<EncounterInfoDto> AvailableEncounters { get; set; } = new();
    public bool HasRequiredEncounter { get; set; }
    public string Message { get; set; }
}

public class EncounterInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public int XPReward { get; set; }
}
