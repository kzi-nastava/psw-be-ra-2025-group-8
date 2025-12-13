using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Author;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Administrator.Administration;

[Route("api/tours/report-problem")]
[ApiController]
public class ReportProblemController : ControllerBase
{
    private readonly IReportProblemService _reportProblemService;
    private readonly ITourService _tourService;

    public ReportProblemController(IReportProblemService reportProblemService, ITourService tourService)
    {
        _reportProblemService = reportProblemService;
        _tourService = tourService;
    }

    [Authorize]
    [HttpGet]
    public ActionResult<PagedResult<ReportProblemDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
        => Ok(_reportProblemService.GetPaged(page, pageSize));

    // Turista prijavljuje problem
    [Authorize(Policy = "touristPolicy")]
    [HttpPost]
    public ActionResult<ReportProblemDto> Create([FromBody] ReportProblemDto problem)
        => Ok(_reportProblemService.Create(problem));

    [Authorize]
    [HttpPut("{id:long}")]
    public ActionResult<ReportProblemDto> Update(long id, [FromBody] ReportProblemDto problem)
    {
        problem.Id = (int)id;
        return Ok(_reportProblemService.Update(problem));
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _reportProblemService.Delete(id);
        return Ok();
    }

    // Autor ili administrator odgovara na problem
    [Authorize(Policy = "authorAdminPolicy")]
    [HttpPost("respond/{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ReportProblemDto), 200)]
    public ActionResult<ReportProblemDto> AuthorRespond(long id, [FromBody] AuthorResponseRequest request)
    {
        var problem = _reportProblemService.GetById((int)id);
        if (!CanAccessProblem(problem))
        {
            return Forbid();
        }
   
        var authorId = GetAuthorIdFromToken();
        var result = _reportProblemService.AuthorRespond((int)id, authorId, request.Response);
        return Ok(result);
    }

    // Turista označava da li je problem rešen
    [Authorize(Policy = "touristPolicy")]
    [HttpPost("resolve/{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ReportProblemDto), 200)]
    public ActionResult<ReportProblemDto> MarkResolved(long id, [FromBody] TouristResolutionRequest request)
    {
        var result = _reportProblemService.MarkResolved((int)id, request.IsResolved, request.Comment);
        return Ok(result);
    }

    // Dobijanje detalja problema sa porukama (samo turista koji je prijavio, autor ili admin)
    [Authorize]
    [HttpGet("{id:long}")]
    public ActionResult<ReportProblemDto> GetById(long id)
    {
        var problem = _reportProblemService.GetById((int)id);
        if (!CanAccessProblem(problem))
        {
            return Forbid();
        }
        return Ok(problem);
    }

    // Dodavanje poruke (omogućeno turistu, autoru i administratoru)
    [Authorize]
    [HttpPost("{id:long}/messages")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IssueMessageDto), 200)]
    public ActionResult<IssueMessageDto> AddMessage(long id, [FromBody] AddMessageRequest request)
    {
        var problem = _reportProblemService.GetById((int)id);
        if (!CanAccessProblem(problem))
        {
            return Forbid();
        }
        
        var authorId = GetUserIdFromToken();
        var result = _reportProblemService.AddMessage((int)id, authorId, request.Content);
        return Ok(result);
    }

    // Dobijanje svih poruka za problem (samo turista koji je prijavio, autor ili admin)
    [Authorize]
    [HttpGet("{id:long}/messages")]
    public ActionResult<List<IssueMessageDto>> GetMessages(long id)
    {
        var problem = _reportProblemService.GetById((int)id);
        if (!CanAccessProblem(problem))
        {
            return Forbid();
        }
        return Ok(_reportProblemService.GetMessages((int)id));
    }

    private int GetUserIdFromToken()
    {
        if (!TryGetUserId(out var userId))
        {
            throw new UnauthorizedAccessException("Unable to determine user ID from token");
        }
        return (int)userId;
    }

    private int GetAuthorIdFromToken()
    {
        if (!TryGetUserId(out var userId))
        {
            throw new UnauthorizedAccessException("Unable to determine user ID from token");
        }
        return (int)userId;
    }

    // Provera da li korisnik može da pristupi problemu
    private bool CanAccessProblem(ReportProblemDto problem)
    {
        // UserId korisnika koji je ulogovan
        if (!TryGetUserId(out var userId))
        {
            return false;
        }
        
        // Administrator može da pristupi svemu
        if (User.IsInRole("administrator"))
        {
            return true;
        }
        
        // Turista koji je prijavio problem
        if (problem.TouristId == userId)
        {
            return true;
        }
        
        // Autor ture
        var tour = _tourService.GetByAuthor((int)userId).FirstOrDefault(t => t.Id == problem.TourId);
        if (tour != null)
        {
            return true;
        }
        
        return false;
    }

    private bool TryGetUserId(out long userId)
    {
        userId = 0;
        var claim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null) return false;
        return long.TryParse(claim.Value, out userId);
    }

    public class AuthorResponseRequest
    {
        public string Response { get; set; }
    }

    public class TouristResolutionRequest
    {
        public bool IsResolved { get; set; }
        public string? Comment { get; set; }
    }

    public class AddMessageRequest
    {
        public string Content { get; set; }
    }
}

