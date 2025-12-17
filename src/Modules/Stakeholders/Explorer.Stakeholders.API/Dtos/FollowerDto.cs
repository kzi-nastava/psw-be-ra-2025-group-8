namespace Explorer.Stakeholders.API.Dtos;

public class FollowerDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; }  
    public DateTime FollowedAt { get; set; }
}
