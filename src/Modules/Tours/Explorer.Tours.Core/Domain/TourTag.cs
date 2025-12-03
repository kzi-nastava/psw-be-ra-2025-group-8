using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class TourTag
    {
        public long TourId { get; set; }
        public Tour? Tour { get; set; }

        public long TagsId { get; set; }
        public Tags? Tags { get; set; }
    }
}
