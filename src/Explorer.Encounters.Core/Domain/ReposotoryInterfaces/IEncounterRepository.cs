using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.ReposotoryInterfaces
{
    public interface IEncounterRepository
    {
        Encounter? GetById(long id);
        Encounter Create(Encounter encounter);
        Encounter Update(Encounter encounter);
        IEnumerable<Encounter> GetAll();
        void Delete(long id);
    }
}
