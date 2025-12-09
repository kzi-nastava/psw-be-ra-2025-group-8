namespace Explorer.Tours.API.Dtos;

public class KeyPointSecretDto
{
    public int Order { get; set; }
    public string Secret { get; set; }
    public DateTime UnlockedAt { get; set; }
}
