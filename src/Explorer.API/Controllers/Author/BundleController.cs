using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/bundles")]
[ApiController]
public class BundleController : ControllerBase
{
    private readonly IBundleService _bundleService;

    public BundleController(IBundleService bundleService) => _bundleService = bundleService;

    [HttpGet]
    public ActionResult<List<BundleDto>> GetMyBundles()
        => Ok(_bundleService.GetByAuthor(GetAuthorIdFromToken()));

    [HttpGet("{id:long}")]
    public ActionResult<BundleDto> GetById(long id)
        => Ok(_bundleService.GetById(id, GetAuthorIdFromToken()));

    [HttpPost]
    public ActionResult<BundleDto> Create([FromBody] CreateBundleDto dto)
        => Ok(_bundleService.Create(dto, GetAuthorIdFromToken()));

    [HttpPut("{id:long}")]
    public ActionResult<BundleDto> Update(long id, [FromBody] UpdateBundleDto dto)
        => Ok(_bundleService.Update(id, dto, GetAuthorIdFromToken()));

    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        _bundleService.Delete(id, GetAuthorIdFromToken());
        return Ok();
    }

    [HttpPut("{id:long}/publish")]
    public ActionResult<BundleDto> Publish(long id)
        => Ok(_bundleService.Publish(id, GetAuthorIdFromToken()));

    [HttpPut("{id:long}/archive")]
    public ActionResult<BundleDto> Archive(long id)
        => Ok(_bundleService.Archive(id, GetAuthorIdFromToken()));

    private int GetAuthorIdFromToken()
    {
        var idClaim = User.FindFirst("id")
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)
                   ?? User.FindFirst("personId")
                   ?? User.FindFirst("sub");

        if (idClaim != null && int.TryParse(idClaim.Value, out int authorId)) return authorId;
        throw new UnauthorizedAccessException("Unable to determine user ID from token");
    }
}

