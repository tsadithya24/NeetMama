using NeetMama.Models;
using System.Text;
using System.Text.Json;

namespace NeetMama.Services
{

    public class PdfChunkResponse
    {
        public int Chunk_Number { get; set; }

        public string Text { get; set; } = string.Empty;

        public int Length { get; set; }
    }
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

        public async Task<string> StoreChunksAsync(
        int documentId,
        string title,
        string subject,
        List<PdfChunkResponse> chunks)
        {
            string baseUrl = _configuration["AISettings:BaseUrl"]!;

            var payload = new
            {
                document_id = documentId,
                title = title,
                subject = subject,
                chunks = chunks.Select(c => new
                {
                    chunk_number = c.Chunk_Number,
                    text = c.Text,
                    length = c.Length
                }).ToList()
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{baseUrl}/store-chunks",
                content);

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

        public async Task<AIFlashCardResponse?> GenerateFlashCardsAsync(
        string subject,
        string topic,
        string cardType,
        int count)
        {
            string baseUrl = _configuration["AISettings:BaseUrl"]!;

            var payload = new
            {
                subject,
                topic,
                card_type = cardType,
                count,
                top_k = 3
            };

            string json = JsonSerializer.Serialize(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{baseUrl}/generate-flashcards",
                content);

            string responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Mama.AI error: {response.StatusCode}. Details: {responseJson}");
            }

            return JsonSerializer.Deserialize<AIFlashCardResponse>(
                responseJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public async Task<AIQuestionResponse?> GenerateQuestionsAsync(
            string subject,
            string topic,
            string difficulty,
            string questionType,
            int count)
        {
            string baseUrl =
                _configuration["AISettings:BaseUrl"]!;

            var payload = new
            {
                subject,
                topic,
                difficulty,
                question_type = questionType,
                count,
                top_k = 3
            };

            string json =
                JsonSerializer.Serialize(payload);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response =
                await _httpClient.PostAsync(
                    $"{baseUrl}/generate-questions",
                    content);

            response.EnsureSuccessStatusCode();

            string responseJson =
                await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<AIQuestionResponse>(
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

        public int Chunk_Count { get; set; }

        public string Full_Text { get; set; } = string.Empty;

        public List<PdfChunkResponse> Chunks { get; set; } = new();

        public string Error { get; set; } = string.Empty;
    }
}