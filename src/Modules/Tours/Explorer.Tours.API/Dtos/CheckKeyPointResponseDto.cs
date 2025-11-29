namespace Explorer.Tours.API.Dtos;

public class CheckKeyPointResponseDto
{
    public bool KeyPointReached { get; set; }
    public int? KeyPointOrder { get; set; }
    public DateTime? ReachedAt { get; set; }
    public DateTime LastActivity { get; set; }
}
