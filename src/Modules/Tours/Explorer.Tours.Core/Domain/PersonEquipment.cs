using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public class PersonEquipment : Entity
    {
        public long PersonId { get; set; }

        public long EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}
