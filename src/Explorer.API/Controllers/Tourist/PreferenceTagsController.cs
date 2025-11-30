using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/preferences/tags")]
    [ApiController]
    public class PreferenceTagsController : ControllerBase
    {
        private readonly IPreferenceTagsService _service;

        public PreferenceTagsController(IPreferenceTagsService service)
        {
            _service = service;
        }

        // GET: api/tourist/preferences/tags
        [HttpGet]
        public ActionResult<IEnumerable<TagDto>> GetMyTags()
        {
            var personId = User.PersonId(); // ClaimsPrincipalExtensions.PersonId()
            var tags = _service.GetTagsForPerson(personId);
            return Ok(tags);
        }

        // POST: api/tourist/preferences/tags
        [HttpPost]
        public ActionResult<TagDto> AddTag([FromBody] TagDto dto)
        {
            var personId = User.PersonId();

            try
            {
                var result = _service.AddTagForPerson(personId, dto);
                if (result == null) return NotFound(); // turist nema preference
                return Ok(result);
            }
            catch (System.ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // http delete brise samo taj tag iz preferenci, tag ostaje u bp u tabeli Tags
        // DELETE: api/tourist/preferences/tags/{tagId}
        [HttpDelete("{tagId:long}")]
        public IActionResult RemoveTag(long tagId)
        {
            var personId = User.PersonId();
            _service.RemoveTagFromPerson(personId, tagId);
            return NoContent();
        }
    }
}

