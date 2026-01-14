using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public
{
    public interface IEncounterService
    {
        List<EncounterDto> GetAllEncounters();
        EncounterDto GetEncounterById(long id);
        List<EncounterDto> GetNearbyEncounters(long personId);
        EncounterDto CreateEncounter(EncounterDto createDto, bool skipLevelCheck = false);
        EncounterDto UpdateEncounter(long id, EncounterUpdateDto updateDto);
        void DeleteEncounter(long id);
        EncounterDto PublishEncounter(long id);
        EncounterDto ArchiveEncounter(long id);
        EncounterDto ReactivateEncounter(long id);
        EncounterDto ApproveEncounter(long id); // admin approves pending encounter
    }
}