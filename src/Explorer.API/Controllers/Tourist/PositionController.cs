using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/tourist/position")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _service;

        public PositionController(IPositionService service)
        {
            _service = service;
        }

        [HttpGet("{touristId}")]
        public ActionResult<PositionDto> Get(int touristId)
        {
            var position = _service.GetByTouristId(touristId);
            if (position == null)
            {
                return NotFound($"Position for tourist {touristId} not found.");
            }
            return Ok(position);
        }

        [HttpPost]
        public ActionResult<PositionDto> Create(PositionDto position)
        {
            try
            {
                var created = _service.CreatePosition(position);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public ActionResult<PositionDto> Update(PositionDto position)
        {
            try
            {
                var updated = _service.UpdatePosition(position);
                return Ok(updated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
