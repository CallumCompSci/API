using System.Net.Http.Headers;
using System.Text;

using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using System.Text.Json;
using Last_Api.Models;







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




        [Fact]
        public async Task Artifacts_returns()
        {
            var username = "StephenB@gmail.com";
            var password = "sit331password";
            var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    
            
            var response = await _client.GetAsync("/api/artifacts");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var artifacts = JsonSerializer.Deserialize<List<Artifact>>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            Assert.NotNull(artifacts);
            Assert.IsType<List<Artifact>>(artifacts);
        }
    }
}
