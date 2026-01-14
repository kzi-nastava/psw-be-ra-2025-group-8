using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Internal;

public interface IInternalEncounterService
{
    EncounterDto GetEncounterById(long id);
}
