namespace Explorer.Stakeholders.API.Dtos;

public class AdminCreateAccountDto
{
    public string Username { get; set; }
    public string Password { get; set; }   // admin sets initial password
    public string Email { get; set; }
    public string Name { get; set; }       // optional 
    public string Surname { get; set; }    // optional 
    public string Role { get; set; }       // Admin or Author
}
