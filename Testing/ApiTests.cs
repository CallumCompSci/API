using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Last.Api.Tests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseContentRoot(Directory.GetCurrentDirectory());
            }).CreateClient();
        }

        [Fact]
        public async Task Api_Starts_Successfully()
        {
            var response = await _client.GetAsync("/");
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound);
        }
    }
}
