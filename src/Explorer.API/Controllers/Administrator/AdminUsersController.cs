using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator;

[Route("api/admin/users")]
[ApiController]
[Authorize(Policy = "administratorPolicy")]  // Only admins can access this controller
public class AdminUsersController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AdminUsersController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    // -------------------------
    // CREATE
    // -------------------------
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<AccountDto> CreateAccount([FromBody] AdminCreateAccountDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = _authService.CreateAccountByAdmin(dto);

        // Use GetById for CreatedAtAction
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // -------------------------
    // READ ALL
    // -------------------------
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AccountOverviewDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<AccountOverviewDto>> GetAll()
    {
        return Ok(_authService.GetAccounts());
    }

    // -------------------------
    // READ BY ID
    // -------------------------
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<AccountDto> GetById(long id)
    {
        var account = _authService.GetById(id);
        if (account == null) return NotFound();

        return Ok(account);
    }

    // -------------------------
    // SET ACTIVE / INACTIVE
    // -------------------------
    public class ActiveStateDto
    {
        public bool IsActive { get; set; }
    }

    [HttpPatch("{id}/active")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SetActiveState(long id, [FromBody] ActiveStateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _authService.ChangeAccountActivation(id, dto.IsActive);
        return NoContent();
    }
}
