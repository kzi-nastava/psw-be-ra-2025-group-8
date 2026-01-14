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

    // New: experience and level
    public int Experience { get; private set; }
    public int Level { get; private set; }

    public Person(long userId, string name, string surname, string email, string? profilePicture = null, string? bio = null, string? motto = null)
    {
        UserId = userId;
        Name = name;
        Surname = surname;
        Email = email;
        ProfilePicture = profilePicture;
        Bio = bio;
        Motto = motto;
        Experience = 0;
        Level = 1; // start at level 1
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

    // Add experience and handle level up. Returns number of levels gained (0 or more).
    public int AddExperience(int xp)
    {
        if (xp <= 0) return 0;

        Experience += xp;
        var levelsGained = 0;

        // Simple progression: XP required for next level = 100 * current Level
        while (Experience >= RequiredXpForNextLevel(Level))
        {
            Experience -= RequiredXpForNextLevel(Level);
            Level++;
            levelsGained++;
        }

        return levelsGained;
    }

    private static int RequiredXpForNextLevel(int currentLevel)
    {
        // Example formula: 100 * currentLevel (level 1 -> 100 XP, level 2 -> 200 XP, ...)
        return 100 * currentLevel;
    }

    private void Validate()
    {
        if (UserId == 0) throw new ArgumentException("Invalid UserId");
        //if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        //if (string.IsNullOrWhiteSpace(Surname)) throw new ArgumentException("Invalid Surname");
        if (!MailAddress.TryCreate(Email, out _)) throw new ArgumentException("Invalid Email");
    }
}