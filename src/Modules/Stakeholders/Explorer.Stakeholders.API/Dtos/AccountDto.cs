namespace Explorer.Stakeholders.API.Dtos;

//Account DTO without password
public class AccountDto
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
}
