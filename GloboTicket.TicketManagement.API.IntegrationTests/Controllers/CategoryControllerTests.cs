using GloboTicket.TicketManagement.Api;
using GloboTicket.TicketManagement.API.IntegrationTests.Base;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GloboTicket.TicketManagement.API.IntegrationTests.Controllers
{

    public class CategoryControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;

        public CategoryControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ReturnsSuccessResult()
        {
            // phase I: create the http client
            var client = await _factory.GetAuthenticatedClientAsync();

            //phase II: do the actual http request that requires authentication
            var response = await client.GetAsync("/api/category/all");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<CategoryListVm>>(responseString);
            
            Assert.IsType<List<CategoryListVm>>(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task ReturnsSuccessResultWithEvents()
        {
            // phase I: create the http client
            var client = await _factory.GetAuthenticatedClientAsync();

            //phase II: do the actual http request that requires authentication
            var response = await client.GetAsync("/api/category/allwithevents");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<CategoryEventListVm>>(responseString);
            
            Assert.IsType<List<CategoryEventListVm>>(result);
            Assert.NotEmpty(result);

            var  concertsFound = false;
            foreach(CategoryEventListVm eventlist in result) 
            {
                if(eventlist.Name.Equals("Concerts")) 
                {
                    concertsFound = true;
                    Assert.NotEmpty(eventlist.Events);
                    break;
                }
            }
            Assert.True(concertsFound);
        }
    }
}
