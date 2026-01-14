using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Tourist;

[Route("api/wallet")]
[ApiController]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>
    /// Turista može da vidi stanje svog nov?anika (authenticated)
    /// </summary>
    [HttpGet("balance")]
    [Authorize(Policy = "touristPolicy")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletDto> GetMyBalance()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var userId = long.Parse(userIdClaim);

        try
        {
            var wallet = _walletService.GetByUserId(userId);
            return Ok(wallet);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get wallet by userId - for testing and internal use
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletDto> GetWalletByUserId([FromQuery] long userId)
    {
        try
        {
            var wallet = _walletService.GetByUserId(userId);
            return Ok(wallet);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
