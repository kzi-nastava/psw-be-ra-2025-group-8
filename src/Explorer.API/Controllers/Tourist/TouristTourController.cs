using Explorer.Tours.Core.UseCases.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [ApiController]
    [Route("api/tours/public")]
    public class TouristTourController : ControllerBase
    {
        private readonly ITouristTourService _touristTourService;

        public TouristTourController(ITouristTourService touristTourService)
        {
            _touristTourService = touristTourService;
        }

        [HttpGet]
        public IActionResult GetPublishedTours()
        {
            return Ok(_touristTourService.GetPublishedTours());
        }

        [HttpGet("{id}")]
        public IActionResult GetPublishedTour(long id)
        {
            return Ok(_touristTourService.GetPublishedTourDetails(id));
        }
    }
}
