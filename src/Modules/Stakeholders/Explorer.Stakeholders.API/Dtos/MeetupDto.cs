namespace Explorer.Stakeholders.API.Dtos
{
    public class MeetupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int CreatorId { get; set; }
    }
}
