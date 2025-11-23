namespace Explorer.Clubs.API.Dtos
{
    public class CreateClubDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
    }
}
