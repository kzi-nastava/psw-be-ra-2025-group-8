using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.User
{
    [Authorize(Policy = "user")]
    [Route("api/meetups")]
    [ApiController]
    public class MeetupController : ControllerBase
    {
        private readonly IMeetupService _service;

        public MeetupController(IMeetupService service)
        {
            _service = service;
        }

        [HttpPost]
        public ActionResult<MeetupDto> Create([FromBody] MeetupDto dto)
        {
            var result = _service.Create(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public ActionResult<MeetupDto> Update(int id, [FromBody] MeetupDto dto)
        {
            dto.Id = id;
            var result = _service.Update(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }

        [HttpGet("{id}")]
        public ActionResult<MeetupDto> Get(int id)
        {
            var result = _service.Get(id);
            return Ok(result);
        }

        [HttpGet("creator/{creatorId}")]
        public ActionResult<IEnumerable<MeetupDto>> GetByCreator(long creatorId)

        {
            var result = _service.GetByCreator(creatorId);
            return Ok(result);
        }
    }
}
