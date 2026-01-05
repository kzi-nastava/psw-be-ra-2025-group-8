using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.UseCases;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Explorer.Stakeholders.Infrastructure.Authentication;

public class JwtGenerator : ITokenGenerator
{
    private readonly string _key = Environment.GetEnvironmentVariable("JWT_KEY") ?? "L1uKpZQzI1Yx0+OaS0kXkE7u0n/5Q0U3R5s3FVmXcXU=";
    private readonly string _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "explorer";
    private readonly string _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "explorer-front.com";

    public AuthenticationTokensDto GenerateAccessToken(User user, long personId)
    {
        var authenticationResponse = new AuthenticationTokensDto();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("id", user.Id.ToString()),
            new("username", user.Username),
            new("personId", personId.ToString()),
            // Koristimo string literal "role" umesto ClaimTypes.Role da dobijemo kratak claim name
            new("role", user.GetPrimaryRoleName())
        };

        // Add standard role claim and numeric role id for compatibility with different clients
        try
        {
            // ClaimTypes.Role - textual role name (e.g. "Administrator", "Author", "Tourist")
            claims.Add(new Claim(ClaimTypes.Role, user.GetPrimaryRoleName()));
        }
        catch
        {
            // ignore if mapping fails
        }

        try
        {
            // roleId - numeric value of enum (e.g. 0,1,2)
            claims.Add(new Claim("roleId", ((int)user.Role).ToString()));
        }
        catch
        {
            // ignore
        }

        var jwt = CreateToken(claims, 60 * 24);
        authenticationResponse.Id = user.Id;
        authenticationResponse.AccessToken = jwt;

        return authenticationResponse;
    }

    private string CreateToken(IEnumerable<Claim> claims, double expirationTimeInMinutes)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: DateTime.Now.AddMinutes(expirationTimeInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
