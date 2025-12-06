
namespace Explorer.Tours.API.Dtos
{
    public class TourDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public int AuthorId { get; set; }
        public List<KeyPointDto>? KeyPoints { get; set; }
        public double LengthInKilometers { get; set; }
        public List<TourEquipmentDto>? RequiredEquipment { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<TourTransportTimeDto>? TransportTimes { get; set; }
    }
}