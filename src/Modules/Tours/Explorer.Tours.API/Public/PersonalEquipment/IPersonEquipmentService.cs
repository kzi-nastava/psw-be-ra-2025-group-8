using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Public.PersonalEquipment
{
    public interface IPersonEquipmentService
    {
        PagedResult<EquipmentForPersonDto> GetPagedForPerson(long personId, int page, int pageSize);
        void AddEquipmentToPerson(long personId, long equipmentId);
        void RemoveEquipmentFromPerson(long personId, long equipmentId);
    }
}
