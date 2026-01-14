using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Route("api/tourist/tours")]
    [ApiController]
    public class TouristSaleController : ControllerBase
    {
        private readonly ITouristTourService _touristTourService;

        public TouristSaleController(ITouristTourService touristTourService)
        {
            _touristTourService = touristTourService;
        }

        [HttpGet("on-sale")]
        public ActionResult<List<TouristTourPreviewDto>> GetToursOnSale([FromQuery] bool sortByDiscount = false)
        {
            var result = _touristTourService.GetToursOnSale(sortByDiscount);
            return Ok(result);
        }
    }
}
