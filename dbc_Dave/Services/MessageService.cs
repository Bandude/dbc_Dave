using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using dbc_Dave.Services.Interfaces;
using dbc_Dave.Data.Models;
using Microsoft.Extensions.Logging;

namespace dbc_Dave.Services
{
   
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public MessageService(string apiKey, ILogger logger)
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

        // Existing methods from the previous example...

        // New methods for handling messages
        public async Task<MessageResponse> CreateMessageAsync(string threadId, CreateMessageRequest createMessageRequest)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(createMessageRequest);
                var response = await _httpClient.PostAsync($"threads/{threadId}/messages", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MessageResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating a message in thread {ThreadId}", threadId);
                throw;
            }
        }

        public async Task<MessageResponse> RetrieveMessageAsync(string threadId, string messageId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"threads/{threadId}/messages/{messageId}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MessageResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving message {MessageId} from thread {ThreadId}", messageId, threadId);
                throw;
            }
        }

        public async Task<MessageResponse> ModifyMessageAsync(string threadId, string messageId, ModifyMessageRequest modifyRequest)
        {
            try
            {
                var jsonRequest = JsonSerializer.Serialize(modifyRequest);
                var response = await _httpClient.PostAsync($"threads/{threadId}/messages/{messageId}", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MessageResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while modifying message {MessageId} in thread {ThreadId}", messageId, threadId);
                throw;
            }
        }

        public async Task<MessageListResponse> ListMessagesAsync(string threadId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"threads/{threadId}/messages");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<MessageListResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while listing messages in thread {ThreadId}", threadId);
                throw;
            }
        }

    
    }
}