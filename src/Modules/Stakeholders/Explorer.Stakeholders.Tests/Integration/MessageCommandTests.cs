using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Explorer.Stakeholders.API.Dtos;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration
{
    public class MessageCommandTests : BaseStakeholdersIntegrationTest
    {
        public MessageCommandTests(StakeholdersTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Get_conversation_returns_seeded_messages_between_users_1_and_2()
        {
            // Act
            // Pretpostavka: test user ima Id = 1, a drugi korisnik je 2
            var response = await Client.GetAsync("/api/messages/2");

            // Assert HTTP odgovor
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var list = await response.Content.ReadFromJsonAsync<List<MessageDto>>();
            list.ShouldNotBeNull();
            list!.Count.ShouldBeGreaterThan(0);

            // Znamo da smo u d-messages.sql ubacili -1 i -2 za (1,2) i (2,1)
            list.ShouldContain(m => m.Id == -1 && m.SenderId == 1 && m.RecipientId == 2);
            list.ShouldContain(m => m.Id == -2 && m.SenderId == 2 && m.RecipientId == 1);
        }
    }
}
