using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
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
    private readonly IEncounterParticipationService _encounterParticipationService;

    public TourExecutionController(
        ITourExecutionService tourExecutionService,
    IEncounterParticipationService encounterParticipationService)
    {
        _tourExecutionService = tourExecutionService;
        _encounterParticipationService = encounterParticipationService;
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
        try
        {
            var result = _tourExecutionService.Create(tourExecution);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            // Tourist already has active tour or invalid input
            return BadRequest(new { message = ex.Message });
        }
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

    [HttpPost("check-encounters-at-keypoint")]
    public ActionResult<AvailableEncountersAtKeyPointDto> CheckEncountersAtKeyPoint([FromBody] CheckEncountersAtKeyPointRequestDto request)
    {
        try
        {
            var result = _tourExecutionService.CheckEncountersAtKeyPoint(request);
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

    [HttpGet("{tourExecutionId:long}/keypoint/{order:int}/secret")]
    public ActionResult<KeyPointSecretDto> GetKeyPointSecret(long tourExecutionId, int order)
    {
        try
        {
            // Security check: verify TourExecution belongs to logged-in tourist
            var touristId = GetTouristIdFromToken();
        
            var tourExecution = _tourExecutionService.Get((int)tourExecutionId);
            if (tourExecution == null)
                return NotFound(new { message = "TourExecution not found" });
      
            if (tourExecution.IdTourist != touristId)
                return Forbid(); // 403 - not your tour execution

            // Get secret (will throw exception if not unlocked)
            var result = _tourExecutionService.GetKeyPointSecret(tourExecutionId, order);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            // KeyPoint not reached yet
            return StatusCode(403, new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("my-active-tour")]
    public ActionResult<TourExecutionDto> GetMyActiveTour()
    {
        var touristId = GetTouristIdFromToken();

        var executions = _tourExecutionService.GetByTourist(touristId);
        var activeTour = executions.FirstOrDefault(te => te.Status == "InProgress");
    
        if (activeTour == null)
            return NotFound(new { message = "No active tour found" });
        
        return Ok(activeTour);
    }

    [HttpGet("{tourExecutionId:long}/weather/current")]
    public ActionResult<WeatherCurrentDto> GetCurrentWeather(
    long tourExecutionId,
    [FromQuery] double? lat = null,
    [FromQuery] double? lon = null)
    {
        var touristId = GetTouristIdFromToken();

        var tourExecution = _tourExecutionService.Get((int)tourExecutionId);
        if (tourExecution == null)
            return NotFound(new { message = "TourExecution not found" });

        if (tourExecution.IdTourist != touristId)
            return Forbid();

        // opcione koordinate koje frontend šalje (trenutna pozicija na mapi)
        if (lat.HasValue && (lat < -90 || lat > 90))
            return BadRequest(new { message = "lat must be between -90 and 90." });

        if (lon.HasValue && (lon < -180 || lon > 180))
            return BadRequest(new { message = "lon must be between -180 and 180." });

        var result = _tourExecutionService.GetCurrentWeather(tourExecutionId, lat, lon);
        if (result == null)
            return NotFound(new { message = "Weather not available" });

        return Ok(result);
    }


    [HttpGet("{tourExecutionId:long}/weather/next-keypoint")]
    public ActionResult<WeatherHourlyForecastDto> GetNextKeyPointWeather(long tourExecutionId, [FromQuery] int hours = 6)
    {
        if (hours < 1 || hours > 48)
            return BadRequest(new { message = "hours must be between 1 and 48." });

        var touristId = GetTouristIdFromToken();

        var tourExecution = _tourExecutionService.Get((int)tourExecutionId);
        if (tourExecution == null)
            return NotFound(new { message = "TourExecution not found" });

        if (tourExecution.IdTourist != touristId)
            return Forbid();

        var result = _tourExecutionService.GetNextKeyPointHourlyForecast(tourExecutionId, hours);
        if (result == null)
            return NotFound(new { message = "Next keypoint not found or weather not available." });

        return Ok(result);
    }



    [HttpPost("{tourExecutionId:long}/activate-encounter/{encounterId:long}")]
    public ActionResult<EncounterParticipationDto> ActivateEncounter(long tourExecutionId, long encounterId)
    {
 try
        {
            // Security check: verify TourExecution belongs to logged-in tourist
            var touristId = GetTouristIdFromToken();
        
  var tourExecution = _tourExecutionService.Get((int)tourExecutionId);
            if (tourExecution == null)
                return NotFound(new { message = "TourExecution not found" });
      
            if (tourExecution.IdTourist != touristId)
     return Forbid(); // 403 - not your tour execution

    // Activate encounter
var activateDto = new ActivateEncounterDto
            {
           PersonId = touristId,
       EncounterId = encounterId
       };

       var result = _encounterParticipationService.ActivateEncounter(activateDto);
            return Ok(result);
  }
        catch (InvalidOperationException ex)
        {
 return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
  {
        return NotFound(new { message = ex.Message });
        }
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