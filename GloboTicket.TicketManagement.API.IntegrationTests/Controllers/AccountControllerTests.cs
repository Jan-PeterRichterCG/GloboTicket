using GloboTicket.TicketManagement.Api;
using GloboTicket.TicketManagement.API.IntegrationTests.Base;
using GloboTicket.TicketManagement.Application.Models.Authentication;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;

namespace GloboTicket.TicketManagement.API.IntegrationTests.Controllers
{

    public class AccountControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public AccountControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task UnsuccessfulLogin()
        {
            var client = _factory.GetAnonymousClient();

            var authenticationRequest = new AuthenticationRequest();
            authenticationRequest.Email = "foo@bar.com"; 
            authenticationRequest.Password = "123456";

            var request = JsonConvert.SerializeObject(authenticationRequest);

            var response = await client.PostAsync("/api/account/authenticate", new StringContent(request, System.Text.Encoding.UTF8, "application/json"));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            var responseString = await response.Content.ReadAsStringAsync();

            Assert.Equal("{\"error\":\"User with foo@bar.com not found.\"}", responseString);
        }

        [Fact]
        public async Task SuccessfulLogin()
        {
            var client = _factory.GetAnonymousClient();

            var authenticationRequest = new AuthenticationRequest();
            authenticationRequest.Email = "john@test.com"; 
            authenticationRequest.Password = "Plural&01?";

            var request = JsonConvert.SerializeObject(authenticationRequest);

            var response = await client.PostAsync("/api/account/authenticate", new StringContent(request, System.Text.Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(responseString);

            Assert.Equal("971f85c6-2c75-4665-8cd4-c122566f6823",authenticationResponse.Id);
            Assert.Equal("johnsmith", authenticationResponse.UserName);
            Assert.Equal("john@test.com", authenticationResponse.Email);
            Assert.NotEmpty(authenticationResponse.Token);
        }
    }
}
