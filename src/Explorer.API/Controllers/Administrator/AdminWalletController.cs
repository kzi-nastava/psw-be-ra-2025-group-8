using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator;

[Route("api/admin/wallet")]
[ApiController]
[Authorize(Policy = "administratorPolicy")]
public class AdminWalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public AdminWalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    /// <summary>
    /// Administrator može da kreira wallet za korisnika koji ga nema
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<WalletDto> CreateWallet([FromQuery] long userId)
    {
        try
        {
            var wallet = _walletService.CreateWallet(userId);
            return Ok(wallet);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Administrator može da uplati AC bilo kom turisti
    /// </summary>
    [HttpPost("deposit")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletDto> DepositCoins([FromBody] DepositCoinsDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var wallet = _walletService.DepositCoins(dto.UserId, dto.Amount);
            return Ok(wallet);
        }
        catch (EntityValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Administrator može da vidi bilo ?iji wallet
    /// </summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(WalletDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletDto> GetWalletByUserId(long userId)
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
