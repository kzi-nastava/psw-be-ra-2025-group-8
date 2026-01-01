using Explorer.Payments.API.Public;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[ApiController]
[Route("api/tours/bundles/public")]
public class TouristBundleController : ControllerBase
{
    private readonly ITouristBundleService _service;
    private readonly IBundlePurchaseService _bundlePurchaseService;

    public TouristBundleController(ITouristBundleService service, IBundlePurchaseService bundlePurchaseService)
    {
        _service = service;
        _bundlePurchaseService = bundlePurchaseService;
    }

    [HttpGet]
    public IActionResult GetAllPublished() => Ok(_service.GetPublished());

    [HttpGet("{id:long}")]
    public IActionResult GetPublishedById(long id) => Ok(_service.GetPublishedById(id));


    [HttpPost("{id:long}/purchase")]
    public IActionResult Purchase([FromQuery] long userId, long id)
        => Ok(_bundlePurchaseService.PurchasePublishedBundle(userId, id));
}

