using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/tour-execution")]
[ApiController]
public class TourExecutionController : ControllerBase
{
    private readonly ITourExecutionService _tourExecutionService;

    public TourExecutionController(ITourExecutionService tourExecutionService)
    {
        _tourExecutionService = tourExecutionService;
    }

    [HttpGet]
    public ActionResult<PagedResult<TourExecutionDto>> GetAll([FromQuery] int page = 0, [FromQuery] int pageSize = 0)
    {
        var result = _tourExecutionService.GetPaged(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public ActionResult<TourExecutionDto> Get(int id)
    {
        var result = _tourExecutionService.Get(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("tourist/{touristId:int}")]
    public ActionResult<List<TourExecutionDto>> GetByTourist(int touristId)
    {
        var result = _tourExecutionService.GetByTourist(touristId);
        return Ok(result);
    }

    [HttpGet("tour/{tourId:int}")]
    public ActionResult<List<TourExecutionDto>> GetByTour(int tourId)
    {
        var result = _tourExecutionService.GetByTour(tourId);
        return Ok(result);
    }

    [HttpGet("tourist/{touristId:int}/tour/{tourId:int}")]
    public ActionResult<TourExecutionDto> GetByTouristAndTour(int touristId, int tourId)
    {
        var result = _tourExecutionService.GetByTouristAndTour(touristId, tourId);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<TourExecutionDto> Create([FromBody] TourExecutionDto tourExecution)
    {
        var result = _tourExecutionService.Create(tourExecution);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public ActionResult<TourExecutionDto> Update(int id, [FromBody] TourExecutionDto tourExecution)
    {
        tourExecution.Id = id;
        var result = _tourExecutionService.Update(tourExecution);
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        _tourExecutionService.Delete(id);
        return Ok();
    }

    [HttpPost("check-keypoint")]
    public ActionResult<CheckKeyPointResponseDto> CheckKeyPoint([FromBody] CheckKeyPointRequestDto request)
    {
        try
        {
            var result = _tourExecutionService.CheckKeyPoint(request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Explorer.BuildingBlocks.Core.Exceptions.NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{tourExecutionId:long}/reached-keypoints")]
    public ActionResult<List<KeyPointReachedDto>> GetReachedKeyPoints(long tourExecutionId)
    {
        // Security check: verify TourExecution belongs to logged-in tourist
        var touristId = GetTouristIdFromToken();
        
        var tourExecution = _tourExecutionService.Get((int)tourExecutionId);
        if (tourExecution == null)
            return NotFound(new { message = "TourExecution not found" });
        
        if (tourExecution.IdTourist != touristId)
            return Forbid(); // 403 - not your tour execution
        
      var result = _tourExecutionService.GetReachedKeyPoints(tourExecutionId);
        return Ok(result);
    }

    private int GetTouristIdFromToken()
    {
        var idClaim = User.FindFirst("id")
                        ?? User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst("personId")
                        ?? User.FindFirst("sub");

        if (idClaim != null && int.TryParse(idClaim.Value, out int touristId))
        {
            return touristId;
        }

        throw new UnauthorizedAccessException("Unable to determine tourist ID from token");
    }
}