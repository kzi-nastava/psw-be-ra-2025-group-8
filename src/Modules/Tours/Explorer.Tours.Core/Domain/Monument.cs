using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class Monument : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public int YearOfConstruction { get; private set; }
        public MonumentStatus Status { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public Monument(string name, string description, int yearOfConstruction, double latitude, double longitude)
        {
            Name = name;
            Description = description;
            YearOfConstruction = yearOfConstruction;
            Status = MonumentStatus.Active; // Automatically set to Active upon creation
            Latitude = latitude;
            Longitude = longitude;
            Validate();
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name.");
            if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description.");
            if (YearOfConstruction < 1 || YearOfConstruction > DateTime.Now.Year)
                throw new ArgumentException("Invalid Year of Construction.");
            if (Latitude < -90 || Latitude > 90)
                throw new ArgumentException("Invalid Latitude. Must be between -90 and 90.");
            if (Longitude < -180 || Longitude > 180)
                throw new ArgumentException("Invalid Longitude. Must be between -180 and 180.");
        }
    }

    public enum MonumentStatus
    {
        Active,
        Inactive
    }
}
