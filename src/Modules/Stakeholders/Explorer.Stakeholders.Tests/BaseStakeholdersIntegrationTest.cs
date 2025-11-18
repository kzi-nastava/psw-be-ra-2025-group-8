using System.Net.Http;
using Explorer.BuildingBlocks.Tests;
using Xunit;

namespace Explorer.Stakeholders.Tests
{
    public abstract class BaseStakeholdersIntegrationTest : IClassFixture<StakeholdersTestFactory>
    {
        protected readonly HttpClient Client;
        protected readonly StakeholdersTestFactory Factory;

        protected BaseStakeholdersIntegrationTest(StakeholdersTestFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }
    }
}
