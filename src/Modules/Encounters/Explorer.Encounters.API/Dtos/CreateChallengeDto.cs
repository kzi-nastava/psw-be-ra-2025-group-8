namespace Explorer.Encounters.API.Dtos;

public class CreateChallengeDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int XPReward { get; set; }
}
