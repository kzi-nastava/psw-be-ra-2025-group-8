namespace Explorer.Tours.API.Dtos
{
    public class TourRatingImageDto
    {
        public long Id { get; set; }
        public long TourRatingId { get; set; }
        public string Url { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
