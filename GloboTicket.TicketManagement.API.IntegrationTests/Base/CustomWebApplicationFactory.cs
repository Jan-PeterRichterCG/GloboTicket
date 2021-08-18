using GloboTicket.TicketManagement.Persistence;
using GloboTicket.TicketManagement.Application.Models.Authentication;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GloboTicket.TicketManagement.API.IntegrationTests.Base
{
    public class CustomWebApplicationFactory<TStartup>
            : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                services.AddDbContext<GloboTicketDbContext>(options =>
                {
                    options.UseInMemoryDatabase("GloboTicketDbContextInMemoryTest");
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var context = scopedServices.GetRequiredService<GloboTicketDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    context.Database.EnsureCreated();

                    try
                    {
                        Utilities.InitializeDbForTests(context);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"An error occurred seeding the database with test messages. Error: {ex.Message}");
                    }
                };
            });
        }

        public HttpClient GetAnonymousClient()
        {
            return CreateClient();
        }

        // create an authenticated HttpClient using basic authentication credentials and an actual call to the authentication method of the Web API 
        public async Task<HttpClient> GetAuthenticatedClientAsync(String email, String password) 
        {
            var client = GetAnonymousClient();
             //phase II: authenticate the http client
            var authenticationRequest = new AuthenticationRequest();
            authenticationRequest.Email = email; 
            authenticationRequest.Password = password;

            var request = JsonConvert.SerializeObject(authenticationRequest);

            var authResponse = await client.PostAsync("/api/account/authenticate", new StringContent(request, System.Text.Encoding.UTF8, "application/json"));

            authResponse.EnsureSuccessStatusCode();

            var authResponseString = await authResponse.Content.ReadAsStringAsync();
            var authenticationResponse = JsonConvert.DeserializeObject<AuthenticationResponse>(authResponseString);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", authenticationResponse.Token);

            return client;
        }

        // convenience method to use the (only?) test identity John Smith by default.
        public Task<HttpClient> GetAuthenticatedClientAsync() {
            return GetAuthenticatedClientAsync("john@test.com", "Plural&01?");
        }
    }
}
