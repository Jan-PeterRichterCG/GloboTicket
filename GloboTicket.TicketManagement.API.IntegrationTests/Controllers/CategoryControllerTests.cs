using GloboTicket.TicketManagement.Api;
using GloboTicket.TicketManagement.API.IntegrationTests.Base;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesList;
using GloboTicket.TicketManagement.Application.Features.Categories.Queries.GetCategoriesListWithEvents;
using GloboTicket.TicketManagement.Application.Features.Categories.Commands.CreateCateogry;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
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
            var response = await client.GetAsync("/api/category/allwithevents?includeHistory=true");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<CategoryEventListVm>>(responseString);
            
            Assert.IsType<List<CategoryEventListVm>>(result);
            Assert.NotEmpty(result);

            var concertsEventList = result.Find(x => x.Name.Equals("Concerts"));
            Assert.NotNull(concertsEventList);
            Assert.NotEmpty(concertsEventList.Events);
        }

        [Fact]
        public async Task ReturnsSuccessResultWithEventsWithIncludeHistorySetToFalse()
        {
            // phase I: create the http client
            var client = await _factory.GetAuthenticatedClientAsync();

            //phase II: do the actual http request that requires authentication
            var response = await client.GetAsync("/api/category/allwithevents?includeHistory=false");

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<CategoryEventListVm>>(responseString);
            
            Assert.IsType<List<CategoryEventListVm>>(result);
            Assert.NotEmpty(result);

            foreach(CategoryEventListVm category in result) 
            {
                foreach(CategoryEventDto categoryEvent in category.Events) 
                {
                    Assert.False(categoryEvent.Date < System.DateTime.Today);
                }
            }
        }

        [Fact]
        public async Task SuccessfulAddCategory()
        {
            var client = await _factory.GetAuthenticatedClientAsync();

            // phase I: create the category
            var createRequest = new CreateCategoryCommand();
            createRequest.Name = "TestCategory"+System.DateTime.Now.Ticks.ToString();

            var request = JsonConvert.SerializeObject(createRequest);

            var response = await client.PostAsync("/api/category", new StringContent(request, System.Text.Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var createResponse = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(responseString);

            Assert.Equal(createRequest.Name, createResponse.Category.Name);

            // phase II: check whether the category is now part of the list of categories

            var checkResponse = await client.GetAsync("/api/category/all");

            checkResponse.EnsureSuccessStatusCode();

            var checkResponseString = await checkResponse.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<List<CategoryListVm>>(checkResponseString);
            
            Assert.IsType<List<CategoryListVm>>(result);
            Assert.NotEmpty(result);
            var newCategory = result.Find(c => c.Name.Equals(createRequest.Name));
            Assert.NotNull(newCategory);
        }
    }
}
