using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class PreferenceTags
    {
        // ova klasa vezuje tagove za preferencu
        // jedna preferenca ima 0 ili vise tagova

        public long TouristPreferencesId { get; set; }
        public long TagsId { get; set; }
        public TouristPreferences TouristPreferences { get; set; }
        public Tags Tags { get; set; }

        public PreferenceTags() { }

        public PreferenceTags(long tpId, long tId)
        {
            TouristPreferencesId = tpId;
            TagsId = tId;
        }
    }
}
