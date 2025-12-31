using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[ApiController]
[Route("api/tours/bundles/public")]
public class TouristBundleController : ControllerBase
{
    private readonly ITouristBundleService _service;

    public TouristBundleController(ITouristBundleService service) => _service = service;

    [HttpGet]
    public IActionResult GetAllPublished() => Ok(_service.GetPublished());

    [HttpGet("{id:long}")]
    public IActionResult GetPublishedById(long id) => Ok(_service.GetPublishedById(id));
}

