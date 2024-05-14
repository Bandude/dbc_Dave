using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using dbc_Dave.Data.Models;
using dbc_Dave.Services.Interfaces;

namespace dbc_Dave.Services
{
    public class MessageService : IMessageService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _serializerOptions;

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

            // Initialize JSON serializer options once and reuse for all serialization/deserialization
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new PolymorphicConverter<MessageContent>() //pick up here
                }
            };
        }

        // Existing methods...

        // Helper method to deserialize HTTP content to a Message object
        private async Task<Message> DeserializeMessage(HttpContent content)
        {
            var jsonString = await content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Message>(jsonString, _serializerOptions);
        }

        // Generic helper method to perform POST requests and return deserialized response
        private async Task<Message> PostAndDeserializeAsync(string url, object requestBody)
        {
            var jsonRequest = JsonSerializer.Serialize(requestBody, _serializerOptions);
            var response = await _httpClient.PostAsync(url, new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            return await DeserializeMessage(response.Content);
        }

        public async Task<Message> CreateMessageAsync(string threadId, CreateMessageRequest createMessageRequest)
        {
            try
            {
                return await PostAndDeserializeAsync($"threads/{threadId}/messages", createMessageRequest);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating a message in thread {ThreadId}", threadId);
                throw;
            }
        }

        public async Task<Message> RetrieveMessageAsync(string threadId, string messageId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"threads/{threadId}/messages/{messageId}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return await DeserializeMessage(response.Content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving message {MessageId} from thread {ThreadId}", messageId, threadId);
                throw;
            }
        }

        public async Task<Message> ModifyMessageAsync(string threadId, string messageId, ModifyMessageRequest modifyRequest)
        {
            try
            {
                return await PostAndDeserializeAsync($"threads/{threadId}/messages/{messageId}", modifyRequest);
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
                return JsonSerializer.Deserialize<MessageListResponse>(content, _serializerOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while listing messages in thread {ThreadId}", threadId);
                throw;
            }
        }
    }
}