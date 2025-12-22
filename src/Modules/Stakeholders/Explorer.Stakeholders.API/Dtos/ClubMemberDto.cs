namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubMemberDto
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
