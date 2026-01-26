using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IInternalPositionService? _positionService;

    public AuthenticationController(IAuthenticationService authenticationService, IInternalPositionService? positionService = null)
    {
        _authenticationService = authenticationService;
        _positionService = positionService;
    }

    [HttpPost]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        try
        {
            var authTokens = _authenticationService.RegisterTourist(account);
            
            // Kreiraj poziciju za turista nakon uspešne registracije
            if (_positionService != null && !string.IsNullOrWhiteSpace(account.LocationSource))
            {
                try
                {
                    var positionDto = new PositionDto
                    {
                        TouristId = (int)authTokens.Id,
                        Latitude = 0, // Frontend će poslati stvarne koordinate
                        Longitude = 0,
                        LocationSource = account.LocationSource
                    };
                    
                    _positionService.CreatePosition(positionDto);
                }
                catch (Exception ex)
                {
                    // Log ali ne zaustavljaj registraciju
                    Console.WriteLine($"Failed to create position for tourist {authTokens.Id}: {ex.Message}");
                }
            }
            
            return Ok(authTokens);
        }
        catch (EntityValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        try
        {
            var tokens = _authenticationService.Login(credentials);
            return Ok(tokens);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }
    }

    // ✅ NOVI ENDPOINT - Vrati sve korisnike
    [HttpGet]
    public ActionResult<IEnumerable<AccountOverviewDto>> GetAllAccounts()
    {
        try
        {
            var accounts = _authenticationService.GetAccounts();
            return Ok(accounts);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error retrieving accounts.", error = ex.Message });
        }
    }
}