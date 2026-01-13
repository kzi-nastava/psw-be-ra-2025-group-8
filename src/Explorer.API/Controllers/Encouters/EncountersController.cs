using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administration
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

        [Authorize(Policy = "authorAdminPolicy")]
        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] EncounterDto encounter)
        {
            // If caller is administrator or author, skip level checks so admin or author can create Draft/Published directly
            var skipLevelCheck = User.IsInRole("administrator") || User.IsInRole("author");

            var created = _encounterService.CreateEncounter(encounter, skipLevelCheck: skipLevelCheck);
            return Ok(created);
        }

        // --------------------
        // UPDATE
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpPut("{id:long}")]
        public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterUpdateDto encounter)
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

        // --------------------
        // PUBLISH
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpPut("{id:long}/publish")]
        public ActionResult<EncounterDto> Publish(long id)
        {
            var result = _encounterService.PublishEncounter(id);
            return Ok(result);
        }

        // --------------------
        // ARCHIVE
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpPut("{id:long}/archive")]
        public ActionResult<EncounterDto> Archive(long id)
        {
            var result = _encounterService.ArchiveEncounter(id);
            return Ok(result);
        }

        // --------------------
        // REACTIVATE
        // --------------------

        [Authorize(Policy = "administratorPolicy")]
        [HttpPut("{id:long}/reactivate")]
        public ActionResult<EncounterDto> Reactivate(long id)
        {
            var result = _encounterService.ReactivateEncounter(id);
            return Ok(result);
        }

        [Authorize(Policy = "authorAdminPolicy")]
        [HttpPost("upload")]
        public ActionResult<string> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0) return BadRequest("No image uploaded");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "encounter");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            return Ok(fileName);
        }
    }
}