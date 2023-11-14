using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using dbc_Dave.Data.Models; // Unsure about the namespace - replace with the correct one

namespace dbc_Dave.Services
{
    public class ThreadService : IThreadService
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
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
            _logger = logger;
        }

        public async Task<ThreadResponse> CreateThreadAsync()
        {
            try
            {
                var response = await _httpClient.PostAsync("threads", new StringContent("{}", Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ThreadResponse>(content);
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
                return JsonConvert.DeserializeObject<ThreadResponse>(content);
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
                var jsonRequest = JsonConvert.SerializeObject(modifyRequest);
                var response = await _httpClient.PostAsync($"threads/{threadId}", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ThreadResponse>(content);
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
                return JsonConvert.DeserializeObject<DeleteThreadResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting thread {ThreadID}", threadId);
                throw;
            }
        }

        // Assuming there is some sort of Run and ThreadAndRun models being used here.
        public async Task<Run> ThreadAndRunAsync(ThreadAndRun threadAndRun)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(threadAndRun);
                var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("threads/runs", httpContent);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                // Configure JsonSerializerSettings to include your custom converter for Tools.
                var settings = new JsonSerializerSettings
                {
                    Converters = new[] { new ToolConverter() }
                };
                return JsonConvert.DeserializeObject<Run>(content, settings); // Use the settings with custom converter
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating a thread and run.");
                throw;
            }
        }
    }
}