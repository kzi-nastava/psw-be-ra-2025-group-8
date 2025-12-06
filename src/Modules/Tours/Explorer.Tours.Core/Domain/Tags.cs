using Explorer.BuildingBlocks.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain
{
    public class Tags : Entity
    {
        public string? Tag { get; set; }

        public ICollection<PreferenceTags> PreferenceTags { get; set; } = new List<PreferenceTags>();

        public ICollection<TourTag> TourTags { get; set; } = new List<TourTag>();

        public Tags() { }

        public Tags(string tag)
        {
            Tag = tag;
        }
    }
}
