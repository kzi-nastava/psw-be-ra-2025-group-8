using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Encouters
{
    [Route("api/administration/encounters")]
    [ApiController]
    public class EncountersController : ControllerBase
    {
        private readonly IEncounterService _encounterService;

        public EncountersController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
        }

        // --------------------
        // READ
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpGet]
        public ActionResult<List<EncounterDto>> GetAll()
        {
            return Ok(_encounterService.GetAllEncounters());
        }

        [Authorize(Policy = "administratorPolicy")]
        [HttpGet("{id:long}")]
        public ActionResult<EncounterDto> GetById(long id)
        {
            return Ok(_encounterService.GetEncounterById(id));
        }

        // --------------------
        // CREATE
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] EncounterDto encounter)
        {
            var created = _encounterService.CreateEncounter(encounter);
            return Ok(created);
        }

        // --------------------
        // UPDATE
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpPut("{id:long}")]
        public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterDto encounter)
        {
            var updated = _encounterService.UpdateEncounter(id, encounter);
            return Ok(updated);
        }

        // --------------------
        // DELETE 
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _encounterService.DeleteEncounter(id);
            return Ok();
        }
    }
}
