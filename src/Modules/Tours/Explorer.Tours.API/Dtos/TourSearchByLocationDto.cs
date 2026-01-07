namespace Explorer.Tours.API.Dtos
{
    public class TourSearchByLocationDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double DistanceInKilometers { get; set; }
    }
}
