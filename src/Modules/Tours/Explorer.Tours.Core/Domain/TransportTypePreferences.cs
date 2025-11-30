using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum TransportType
    {
        Walk,
        Bicycle,
        Car,
        Boat
    }

    public class TransportTypePreferences : Entity
    {
        public long PreferenceId { get; set; }
        public TransportType Transport { get; set; }
        public int Rating { get; set; }
        public TouristPreferences Preference { get; set; }

        public TransportTypePreferences() { }

        public TransportTypePreferences(long preferenceId, TransportType type, int rate)
        {
            PreferenceId = preferenceId;
            Transport = type;
            Rating = rate;
        }
    }


}
