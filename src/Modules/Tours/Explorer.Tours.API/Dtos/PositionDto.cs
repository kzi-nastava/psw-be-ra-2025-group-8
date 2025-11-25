namespace Explorer.Tours.API.Dtos
{
    public class PositionDto
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TouristId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
