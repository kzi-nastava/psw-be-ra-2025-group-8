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
            var result = _touristTourService.GetPublishedTourDetails(id);
            if (result == null) return NotFound("Tour not found or not published.");
            return Ok(result);
        }

        [HttpGet("{id}/keypoints")]
        public IActionResult GetTourKeyPoints(long id)
        {
            var keyPoints = _touristTourService.GetTourKeyPoints(id);
            if (!keyPoints.Any()) return NotFound("Tour not found or has no keypoints.");
            return Ok(keyPoints);
        }
    }
}
