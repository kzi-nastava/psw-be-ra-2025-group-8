using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/author/tour-equipment")]
[ApiController]
public class TourEquipmentController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourEquipmentController(ITourService tourService)
    {
        _tourService = tourService;
    }

    // =============================
    // GET: List all equipment for a tour
    // GET api/author/tour-equipment/{tourId}
    // =============================
    [HttpGet("{tourId:long}")]
    public ActionResult<List<TourEquipmentDto>> GetEquipment(long tourId)
    {
        var authorId = GetAuthorIdFromToken();

        var tours = _tourService.GetByAuthor(authorId);
        var tour = tours.FirstOrDefault(t => t.Id == tourId);

        if (tour == null)
        {
            return NotFound("Tour not found or you don't have permission to access it.");
        }

        return Ok(tour.RequiredEquipment);
    }

    // =============================
    // POST: Add equipment to tour
    // POST api/author/tour-equipment/{tourId}/{equipmentId}
    // =============================
    [HttpPost("{tourId:long}/{equipmentId:long}")]
    public ActionResult<TourDto> AddEquipment(long tourId, long equipmentId)
    {
        var authorId = GetAuthorIdFromToken();
        var result = _tourService.AddEquipment(tourId, equipmentId, authorId);
        return Ok(result);
    }

    // =============================
    // DELETE: Remove equipment from tour
    // DELETE api/author/tour-equipment/{tourId}/{equipmentId}
    // =============================
    [HttpDelete("{tourId:long}/{equipmentId:long}")]
    public ActionResult<TourDto> RemoveEquipment(long tourId, long equipmentId)
    {
        var authorId = GetAuthorIdFromToken();
        var result = _tourService.RemoveEquipment(tourId, equipmentId, authorId);
        return Ok(result);
    }

    // =============================
    // Helper: Extract authorId from JWT
    // =============================
    private int GetAuthorIdFromToken()
    {
        var idClaim = User.FindFirst("id")
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)
                   ?? User.FindFirst("personId")
                   ?? User.FindFirst("sub");

        if (idClaim != null && int.TryParse(idClaim.Value, out int authorId))
        {
            return authorId;
        }

        throw new UnauthorizedAccessException("Unable to determine user ID from token");
    }
}
