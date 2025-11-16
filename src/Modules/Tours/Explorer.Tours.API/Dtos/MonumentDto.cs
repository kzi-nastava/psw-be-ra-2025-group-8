namespace Explorer.Tours.API.Dtos
{
    public class MonumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int YearOfConstruction { get; set; }
        public string Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
