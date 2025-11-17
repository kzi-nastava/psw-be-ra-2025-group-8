using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "touristPolicy")]
[Route("api/tours/report-problem")]
[ApiController]
public class ReportProblemController : ControllerBase
{
    private readonly IReportProblemService _reportProblemService;

    public ReportProblemController(IReportProblemService reportProblemService)
    {
        _reportProblemService = reportProblemService;
    }

    [HttpGet]
    public ActionResult<PagedResult<ReportProblemDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_reportProblemService.GetPaged(page, pageSize));
    }

    [HttpPost]
    public ActionResult<ReportProblemDto> Create([FromBody] ReportProblemDto problem)
    {
        return Ok(_reportProblemService.Create(problem));
    }

    [HttpPut("{id:long}")]
    public ActionResult<ReportProblemDto> Update(long id, [FromBody] ReportProblemDto problem)
    {
        problem.Id = (int)id;
        return Ok(_reportProblemService.Update(problem));
    }

    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _reportProblemService.Delete(id);
        return Ok();
    }
}