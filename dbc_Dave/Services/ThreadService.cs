using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace dbc_Dave.Services
{
    public class ThreadService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;


        public ThreadService(string apiKey, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key cannot be null or whitespace.", nameof(apiKey));
            }

            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _logger = logger;
        }

        public async Task<ThreadResponse> CreateThreadAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("threads", new StringContent("{}", Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ThreadResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating a thread.");
                throw;
            }
        }

        public async Task<ThreadResponse> RetrieveThreadAsync(string threadId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"threads/{threadId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ThreadResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving thread {ThreadID}", threadId);
                throw;
            }
        }

        public async Task<ThreadResponse> ModifyThreadAsync(string threadId, ModifyThreadRequest modifyRequest)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(modifyRequest);
                var response = await _httpClient.PostAsync($"threads/{threadId}", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<ThreadResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while modifying thread {ThreadID}", threadId);
                throw;
            }
        }

        public async Task<DeleteThreadResponse> DeleteThreadAsync(string threadId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, $"threads/{threadId}");
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<DeleteThreadResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting thread {ThreadID}", threadId);
                throw;
            }
        }

        // Model classes for the JSON responses.
        public class ThreadResponse
        {
            public string Id { get; set; }
            public string Object { get; set; }
            public long CreatedAt { get; set; }
            public Metadata Metadata { get; set; }
        }

        public class DeleteThreadResponse
        {
            public string Id { get; set; }
            public string Object { get; set; }
            public bool Deleted { get; set; }
        }

        public class ModifyThreadRequest
        {
            public Metadata Metadata { get; set; }
        }

        public class Metadata
        {
            public string Modified { get; set; }
            public string User { get; set; }
        }
    }
}