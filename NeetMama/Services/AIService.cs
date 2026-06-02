using System.Text.Json;

namespace NeetMama.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AIService(
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetHealthAsync()
        {
            string baseUrl =
                _configuration["AISettings:BaseUrl"]!;

            var response =
                await _httpClient.GetAsync($"{baseUrl}/health");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}