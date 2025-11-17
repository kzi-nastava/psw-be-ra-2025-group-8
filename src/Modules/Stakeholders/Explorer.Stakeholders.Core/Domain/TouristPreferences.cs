using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public enum DifficultyLevel
    {
        Beginner,
        Intermediate,
        Professional
    }

    public class TouristPreferences : Entity
    {
        public long PersonId { get; set; }
        public Person Person { get; set; }
        public DifficultyLevel Difficulty { get; set; }

        //novo polje za povezivanje
        public ICollection<TransportTypePreferences> TransportTypePreferences { get; set; } = new List<TransportTypePreferences>();
        //public List<TransportTypePreferences> TransportPreferences { get; set; } = new();


        //prazan
        public TouristPreferences() { }

        public TouristPreferences(long personId, DifficultyLevel difficulty)
        {
            PersonId = personId;
            Difficulty = difficulty;
        }
    }
}

