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
            return Ok(_service.GetByTouristId(touristId));
        }

        [HttpPost]
        public ActionResult<PositionDto> Create(PositionDto position)
        {
            return Ok(_service.CreatePosition(position));
        }

        [HttpPut]
        public ActionResult<PositionDto> Update(PositionDto position)
        {
            return Ok(_service.UpdatePosition(position));
        }
    }

}
