using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourEquipment : Entity
    {
        public long TourId { get; private set; }
        public Tour? Tour { get; private set; }

        public long EquipmentId { get; private set; }
        public Equipment? Equipment { get; private set; }

        private TourEquipment() { }

        public TourEquipment(long tourId, long equipmentId)
        {
            TourId = tourId;
            EquipmentId = equipmentId;
        }
    }
}
