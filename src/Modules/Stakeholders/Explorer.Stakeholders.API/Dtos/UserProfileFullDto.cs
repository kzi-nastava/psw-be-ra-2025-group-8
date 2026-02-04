using System.Collections.Generic;

namespace Explorer.Stakeholders.API.Dtos;

public class UserProfileFullDto
{
    public PersonDto Profile { get; set; }
    public List<FollowerDto> Followers { get; set; } = new();
    public List<FollowerDto> Following { get; set; } = new();
    public int FollowersCount => Followers?.Count ?? 0;
    public int FollowingCount => Following?.Count ?? 0;
}
