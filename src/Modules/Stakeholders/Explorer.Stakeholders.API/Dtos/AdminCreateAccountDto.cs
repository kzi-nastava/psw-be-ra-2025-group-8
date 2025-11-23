namespace Explorer.Stakeholders.API.Dtos;

public class AdminCreateAccountDto
{
    public string Username { get; set; }
    public string Password { get; set; }   // admin sets initial password
    public string Email { get; set; }
    public string Role { get; set; }       // Admin or Author
}
