using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/encounters")]
    [ApiController]
    public class EncounterParticipationController : ControllerBase
    {
        private readonly IEncounterService _encounterService;
        private readonly IEncounterParticipationService _participationService;

        public EncounterParticipationController(
            IEncounterService encounterService,
            IEncounterParticipationService participationService)
        {
            _encounterService = encounterService;
            _participationService = participationService;
        }

        // --------------------
        // GET ALL PUBLISHED ENCOUNTERS WITH STATUS
        // --------------------

        [HttpGet]
        public ActionResult<List<EncounterWithStatusDto>> GetAllPublishedWithStatus()
        {
            var personId = GetPersonIdFromToken();

            // Get all published encounters
            var allEncounters = _encounterService.GetAllEncounters()
                .Where(e => e.Status == "Published")
                .ToList();

            // Get user's participations
            var participations = _participationService.GetParticipationsByPerson(personId);
            var participationDict = participations.ToDictionary(p => p.EncounterId);

            // Combine encounters with participation status
            var result = allEncounters.Select(encounter => new EncounterWithStatusDto
            {
                Encounter = encounter,
                ParticipationStatus = participationDict.ContainsKey(encounter.Id)
                    ? participationDict[encounter.Id].Status
                    : "NotStarted",
                ActivatedAt = participationDict.ContainsKey(encounter.Id)
                    ? participationDict[encounter.Id].ActivatedAt
                    : (DateTime?)null,
                CompletedAt = participationDict.ContainsKey(encounter.Id)
                    ? participationDict[encounter.Id].CompletedAt
                    : null,
                XPAwarded = participationDict.ContainsKey(encounter.Id)
                    ? participationDict[encounter.Id].XPAwarded
                    : null
            }).ToList();

            return Ok(result);
        }

        // --------------------
        // GET NEARBY ENCOUNTERS (within 1km)
        // --------------------

        [HttpGet("nearby")]
        public ActionResult<List<EncounterDto>> GetNearbyEncounters()
        {
            var personId = GetPersonIdFromToken();
            var nearbyEncounters = _encounterService.GetNearbyEncounters(personId);
            return Ok(nearbyEncounters);
        }

        // --------------------
        // GET MY ACTIVE ENCOUNTERS
        // --------------------

        [HttpGet("active")]
        public ActionResult<List<EncounterParticipationDto>> GetMyActiveEncounters()
        {
            var personId = GetPersonIdFromToken();
            var activeEncounters = _participationService.GetActiveEncountersByPerson(personId);
            return Ok(activeEncounters);
        }

        // --------------------
        // GET MY PARTICIPATION HISTORY
        // --------------------

        [HttpGet("history")]
        public ActionResult<List<EncounterParticipationDto>> GetMyParticipationHistory()
        {
            var personId = GetPersonIdFromToken();
            var participations = _participationService.GetParticipationsByPerson(personId);
            return Ok(participations);
        }

        // --------------------
        // ACTIVATE ENCOUNTER
        // --------------------

        [HttpPost("{encounterId:long}/activate")]
        public ActionResult<EncounterParticipationDto> ActivateEncounter(long encounterId)
        {
            var personId = GetPersonIdFromToken();
            var activateDto = new ActivateEncounterDto
            {
                PersonId = personId,
                EncounterId = encounterId
            };

            var result = _participationService.ActivateEncounter(activateDto);
            return Ok(result);
        }

        // --------------------
        // COMPLETE ENCOUNTER
        // --------------------

        [HttpPut("{encounterId:long}/complete")]
        public ActionResult<EncounterParticipationDto> CompleteEncounter(long encounterId)
        {
            var personId = GetPersonIdFromToken();
            var completeDto = new CompleteEncounterDto
            {
                PersonId = personId,
                EncounterId = encounterId
            };

            var result = _participationService.CompleteEncounter(completeDto);
            return Ok(result);
        }

        // --------------------
        // ABANDON ENCOUNTER
        // --------------------

        [HttpPut("{encounterId:long}/abandon")]
        public ActionResult<EncounterParticipationDto> AbandonEncounter(long encounterId)
        {
            var personId = GetPersonIdFromToken();
            var result = _participationService.AbandonEncounter(personId, encounterId);
            return Ok(result);
        }

        // --------------------
        // REACTIVATE ENCOUNTER
        // --------------------

        [HttpPut("{encounterId:long}/reactivate")]
        public ActionResult<EncounterParticipationDto> ReactivateEncounter(long encounterId)
        {
            var personId = GetPersonIdFromToken();
            var result = _participationService.ReactivateEncounter(personId, encounterId);
            return Ok(result);
        }

        // --------------------
        // HELPER METHOD
        // --------------------

        private long GetPersonIdFromToken()
        {
            var idClaim = User.FindFirst("id")
                       ?? User.FindFirst(ClaimTypes.NameIdentifier)
                       ?? User.FindFirst("personId")
                       ?? User.FindFirst("sub");

            if (idClaim != null && long.TryParse(idClaim.Value, out long personId))
            {
                return personId;
            }

            throw new UnauthorizedAccessException("Unable to determine user ID from token");
        }
    }
}