namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public long OwnerId { get; set; }
        public string? OwnerUsername { get; set; }
        public List<long> MemberIds { get; set; } = new();
        public List<string>? MemberUsernames { get; set; }
        // new status exposed to API (string)
        public string Status { get; set; } = "Active";
    }
}
