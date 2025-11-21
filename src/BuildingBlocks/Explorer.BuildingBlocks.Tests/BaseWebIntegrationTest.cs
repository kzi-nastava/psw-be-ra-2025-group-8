using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Security.Claims;
using Explorer.API;
using Xunit;
using System.Net.Http;

namespace Explorer.BuildingBlocks.Tests;

public class BaseWebIntegrationTest<TTestFactory> : IClassFixture<TTestFactory> where TTestFactory : WebApplicationFactory<Program>
{
    protected TTestFactory Factory { get; }
    protected HttpClient Client { get; }

    public BaseWebIntegrationTest(TTestFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();    
    }

    protected static ControllerContext BuildContext(string id)
    {
        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("personId", id),
                    // Added NameIdentifier claim so author controllers can resolve current author id
                    new Claim(ClaimTypes.NameIdentifier, id),
                    // za rating(GetLoggedInUserId())
                    new Claim("id", id)
                }))
            }
        };
    }
}
