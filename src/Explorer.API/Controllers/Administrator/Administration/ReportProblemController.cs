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

    // Autor odgovara na problem
    [Authorize(Policy = "authorPolicy")]
    [HttpPost("respond/{id:long}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ReportProblemDto), 200)]
    public ActionResult<ReportProblemDto> AuthorRespond(long id, [FromBody] AuthorResponseRequest request)
    {
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

    // -----------------------------------------
    //  ADMINISTRATORSKE AKCIJE
    // -----------------------------------------

    // Administrator postavlja rok za rešavanje problema
    [Authorize(Roles = "administrator")]
    [HttpPost("{id:long}/deadline")]
    [ProducesResponseType(typeof(ReportProblemDto), 200)]
    public ActionResult<ReportProblemDto> SetDeadline(long id, [FromBody] SetDeadlineRequest request)
    {
        if (request.Deadline <= DateTime.UtcNow)
        {
            return BadRequest("Deadline mora biti u budućnosti.");
        }

        var result = _reportProblemService.SetDeadline((int)id, request.Deadline);
        return Ok(result);
    }

    // Administrator zatvara problem bez kazne
    [Authorize(Roles = "administrator")]
    [HttpPut("{id:long}/close")]
    [ProducesResponseType(typeof(ReportProblemDto), 200)]
    public ActionResult<ReportProblemDto> CloseIssue(long id)
    {
        var result = _reportProblemService.CloseIssueByAdmin((int)id);
        return Ok(result);
    }

    // Administrator penalizuje autora (npr. arhivira turu)
    [Authorize(Roles = "administrator")]
    [HttpPut("{id:long}/penalize")]
    [ProducesResponseType(typeof(ReportProblemDto), 200)]
    public ActionResult<ReportProblemDto> PenalizeAuthor(long id)
    {
        var result = _reportProblemService.PenalizeAuthor((int)id);
        return Ok(result);
    }

    // DTO za unos roka
    public class SetDeadlineRequest
    {
        public DateTime Deadline { get; set; }
    }



    private int GetUserIdFromToken()
    {
        if (!TryGetPersonId(out var personId))
        {
            throw new UnauthorizedAccessException("Unable to determine person ID from token");
        }
        return (int)personId;
    }

    private int GetAuthorIdFromToken()
    {
        if (!TryGetPersonId(out var personId))
        {
            throw new UnauthorizedAccessException("Unable to determine person ID from token");
        }
        return (int)personId;
    }

    // Provera da li korisnik može da pristupi problemu
    private bool CanAccessProblem(ReportProblemDto problem)
    {
        // PersonId korisnika koji je ulogovan
        if (!TryGetPersonId(out var personId))
        {
            return false;
        }
        
        // Administrator može da pristupi svemu
        if (User.IsInRole("administrator"))
        {
            return true;
        }
        
        // Turista koji je prijavio problem
        if (problem.TouristId == personId)
        {
            return true;
        }
        
        // Autor ture
        var tour = _tourService.GetByAuthor((int)personId).FirstOrDefault(t => t.Id == problem.TourId);
        if (tour != null)
        {
            return true;
        }
        
        return false;
    }

    private bool TryGetPersonId(out long personId)
    {
        personId = 0;
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId");
        if (claim == null) return false;
        return long.TryParse(claim.Value, out personId);
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

