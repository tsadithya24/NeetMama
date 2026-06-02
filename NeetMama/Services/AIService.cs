using System.Text;
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
            string baseUrl = _configuration["AISettings:BaseUrl"]!;

            var response = await _httpClient.GetAsync($"{baseUrl}/health");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<PdfExtractResponse?> ExtractPdfTextAsync(string filePath)
        {
            string baseUrl = _configuration["AISettings:BaseUrl"]!;

            var payload = new
            {
                file_path = filePath
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{baseUrl}/extract-pdf-text",
                content);

            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<PdfExtractResponse>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }

    public class PdfExtractResponse
    {
        public bool Success { get; set; }

        public int Page_Count { get; set; }

        public int Text_Length { get; set; }

        public string Full_Text { get; set; } = string.Empty;

        public string Error { get; set; } = string.Empty;
    }
}