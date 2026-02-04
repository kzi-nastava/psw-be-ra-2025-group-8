namespace Explorer.Stakeholders.API.Dtos;

public class UserSearchResultDto
{
    public long UserId { get; set; }
    public long PersonId { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsFollowing { get; set; }
}
