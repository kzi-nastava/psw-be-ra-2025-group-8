using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Route("api/administration/equipment")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;

    }
    [Authorize(Policy = "administratorPolicy")]
    [HttpGet]
    public ActionResult<PagedResult<EquipmentDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_equipmentService.GetPaged(page, pageSize));
    }

    [HttpGet("~/api/author/equipment")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<List<EquipmentDto>> GetAllForAuthor()
    {
        var items = _equipmentService.GetPaged(0, 1000).Results;
        return Ok(items);
    }

    [Authorize(Policy = "administratorPolicy")]
    [HttpPost]
    public ActionResult<EquipmentDto> Create([FromBody] EquipmentDto equipment)
    {
        return Ok(_equipmentService.Create(equipment));
    }

    [Authorize(Policy = "administratorPolicy")]
    [HttpPut("{id:long}")]
    public ActionResult<EquipmentDto> Update(long id, [FromBody] EquipmentDto equipment)
    {
        equipment.Id = (int)id;
        return Ok(_equipmentService.Update(equipment));
    }

    [Authorize(Policy = "administratorPolicy")]
    [HttpDelete("{id:long}")]
    public ActionResult Delete(long id)
    {
        _equipmentService.Delete(id);
        return Ok();
    }
}