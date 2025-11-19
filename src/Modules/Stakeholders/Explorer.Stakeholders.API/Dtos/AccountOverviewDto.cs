namespace Explorer.Stakeholders.API.Dtos;

//account overview DTO for listing accounts (admin UI)
public class AccountOverviewDto
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public string Email { get; set; }
}
