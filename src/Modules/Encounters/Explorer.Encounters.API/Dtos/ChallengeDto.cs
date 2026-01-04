using System;

namespace Explorer.Encounters.API.Dtos;

public class ChallengeDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long CreatorPersonId { get; set; }
    public int Status { get; set; }
    public int XPReward { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
