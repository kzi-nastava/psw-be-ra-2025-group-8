namespace Explorer.Clubs.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; }
        public long OwnerId { get; set; }
        public List<int> MemberIds { get; set; } = new();
    }
}
