namespace Explorer.Tours.API.Dtos
{
    public class KeyPointDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? Secret { get; set; }
        public int Order { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
