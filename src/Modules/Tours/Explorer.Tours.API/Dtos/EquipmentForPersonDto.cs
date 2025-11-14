using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class EquipmentForPersonDto
    {
        public long EquipmentId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        // for checkbox in UI
        public bool IsOwned { get; set; }
    }
}
