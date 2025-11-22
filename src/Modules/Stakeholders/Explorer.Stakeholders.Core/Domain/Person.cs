using Explorer.BuildingBlocks.Core.Domain;
using System.Net.Mail;

namespace Explorer.Stakeholders.Core.Domain;

public class Person : Entity
{
    public long UserId { get; init; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }
    public string? ProfilePicture { get; private set; }
    public string? Bio { get; private set; }
    public string? Motto { get; private set; }

    public Person(long userId, string name, string surname, string email, string? profilePicture = null, string? bio = null, string? motto = null)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Email = email;
        ProfilePicture = profilePicture;
        Bio = bio;
        Motto = motto;
        Validate();
    }

    public void UpdateProfile(string name, string surname, string email, string? profilePicture, string? bio, string? motto)
    {
        Name = name;
        Surname = surname;
        Email = email;
        ProfilePicture = profilePicture;
        Bio = bio;
        Motto = motto;
        Validate();
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid UserId");
        //if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        //if (string.IsNullOrWhiteSpace(Surname)) throw new ArgumentException("Invalid Surname");
        if (!MailAddress.TryCreate(Email, out _)) throw new ArgumentException("Invalid Email");
    }
}