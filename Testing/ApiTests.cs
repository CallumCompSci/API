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

            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJIYXJyeUBnbWFpbC5jb20iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJhZG1pbiIsImV4cCI6MTc1OTE2NDg3MywiaXNzIjoiQ2FsbHVtc0FQSSIsImF1ZCI6IkFQSVVzZXIifQ.cRRW9TdBWwuli88I75HLZM9Hd88GStQldLlIMqxpRww"; 
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        [Fact]
        public async Task Api_Starts_Successfully()
        {
            var response = await _client.GetAsync("/");
            Assert.True(response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound);
        }



        //Artifacts
        [Fact]
        public async Task getArtifacts()
        {
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

        //Eras
        [Fact]
        public async Task getEras()
        {
            var response = await _client.GetAsync("/api/arteras");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var arteras = JsonSerializer.Deserialize<List<ArtEra>>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            Assert.NotNull(arteras);
            Assert.IsType<List<ArtEra>>(arteras);
        }


        //Eras
        [Fact]
        public async Task getTribes()
        {
            var response = await _client.GetAsync("/api/tribe");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var tribes = JsonSerializer.Deserialize<List<Tribe>>(content, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });
            
            Assert.NotNull(tribes);
            Assert.IsType<List<Tribe>>(tribes);
        }

       

        
    }
}
