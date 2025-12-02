using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Administrator.Administration;

[Route("api/tours/report-problem")]
[ApiController]
public class ReportProblemController : ControllerBase
{
    private readonly IReportProblemService _reportProblemService;

    public ReportProblemController(IReportProblemService reportProblemService)
    {
        _reportProblemService = reportProblemService;
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

    public class AuthorResponseRequest
    {
        public string Response { get; set; }
    }

    public class TouristResolutionRequest
    {
        public bool IsResolved { get; set; }
        public string? Comment { get; set; }
    }
}
