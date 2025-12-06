using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourTransportTime
    {
        public long TourId { get; set; }
        public Tour? Tour { get; set; }

        public TransportType Transport { get; set; }
        public int DurationMinutes { get; set; }

        public TourTransportTime()
        {
        }

        public TourTransportTime(long tourId, TransportType transport, int durationMinutes)
        {
            TourId = tourId;
            Transport = transport;
            DurationMinutes = durationMinutes;
        }
    }
}
