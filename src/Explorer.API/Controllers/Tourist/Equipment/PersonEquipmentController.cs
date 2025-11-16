using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.PersonalEquipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Equipment
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/administration/person-equipment")]
    [ApiController]
    public class PersonEquipmentController : ControllerBase
    {
        private readonly IPersonEquipmentService _personEquipmentService;

        public PersonEquipmentController(IPersonEquipmentService service)
        {
            _personEquipmentService = service;
        }

        private bool TryGetPersonId(out long personId)
        {
            personId = 0;
            var claim = User.Claims.FirstOrDefault(c => c.Type == "personId");
            if (claim == null) return false;
            return long.TryParse(claim.Value, out personId);
        }

        [HttpGet]
        public IActionResult GetForPerson([FromQuery] int page, [FromQuery] int pageSize)
        {
            if (!TryGetPersonId(out var personId))
                return Unauthorized();

            var result = _personEquipmentService.GetPagedForPerson(personId, page, pageSize);
            return Ok(result);
        }

        [HttpPost("assign")]
        public IActionResult Assign([FromBody] long equipmentId)
        {
            if (!TryGetPersonId(out var personId))
                return Unauthorized();

            _personEquipmentService.AddEquipmentToPerson(personId, equipmentId);
            return Ok();
        }

        [HttpDelete("unassign")]
        public IActionResult Unassign([FromBody] long equipmentId)
        {
            if (!TryGetPersonId(out var personId))
                return Unauthorized();

            _personEquipmentService.RemoveEquipmentFromPerson(personId, equipmentId);
            return Ok();
        }
    }
}
