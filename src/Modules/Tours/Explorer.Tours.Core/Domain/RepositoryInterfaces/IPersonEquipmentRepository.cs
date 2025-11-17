using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IPersonEquipmentRepository
    {
        IEnumerable<PersonEquipment> GetByPersonId(long personId);
        PersonEquipment? Find(long personId, long equipmentId);
        void Add(PersonEquipment entity);
        void Remove(PersonEquipment entity);
    }
}
