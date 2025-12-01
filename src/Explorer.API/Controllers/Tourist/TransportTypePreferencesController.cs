using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/transport-type-preferences")]
    [ApiController]
    public class TransportTypePreferencesController : ControllerBase
    {
        private readonly ITransportTypePreferencesService _service;

        public TransportTypePreferencesController(ITransportTypePreferencesService service)
        {
            _service = service;
        }

        // URL: GET /api/tourist/transport-type-preferences
        [HttpGet]
        public ActionResult<IEnumerable<TransportTypePreferenceDto>> Get()
        {
            long personId = User.PersonId();
            var list = _service.Get(personId);
            return Ok(list);
        }

        // URL: PUT /api/tourist/transport-type-preferences
        // Body: array of TransportTypePreferenceDto { "transport":"Walk", "rating": 2 }
        [HttpPut]
        public ActionResult Put([FromBody] IEnumerable<TransportTypePreferenceDto> dtos)
        {
            long personId = User.PersonId();
            _service.Update(personId, dtos);
            return Ok();
        }
    }
}
