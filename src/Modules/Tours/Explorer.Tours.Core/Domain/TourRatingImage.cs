using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourRatingImage : Entity
    {
        public long TourRatingId { get; set; }
        public string Url { get; set; }
        public DateTime UploadedAt { get; set; }

        public TourRatingImage(long tourRatingId, string url)
        {
            TourRatingId = tourRatingId;
            Url = url;
            UploadedAt = DateTime.UtcNow;
            Validate();
        }

        public TourRatingImage() { }

        private void Validate()
        {
            if (TourRatingId <= 0)
                throw new ArgumentException("TourRatingId must be greater than zero.");
            if (string.IsNullOrWhiteSpace(Url))
                throw new ArgumentException("Url cannot be empty.");
        }
    }
}
