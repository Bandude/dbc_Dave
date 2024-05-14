using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using dbc_Dave.Data.Models;
using dbc_Dave.Services.Interfaces;
using Newtonsoft.Json;

namespace dbc_Dave.Services
{
   

        public class AssistantService : IAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger _logger;

        public AssistantService(string apiKey, ILogger logger)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/"),
       
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
            _logger = logger;

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("API key cannot be null or whitespace.", nameof(apiKey));
            }


        }

        public async Task<Assistant> CreateAssistantAsync(string model, string name, string instructions, List<Tool>? tools)
        {
            try
            {
                var payload = new { model, name, instructions, tools };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync("assistants", content);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating assistant {e}", e);
                throw;
            }
        }

        public async Task<Assistant> RetrieveAssistantAsync(string assistantId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"assistants/{assistantId}");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieveing an assistant  {e}", e);
                throw;
            }
        }

        public async Task<Assistant> ModifyAssistantAsync(string assistantId, string instructions)
        {
            try
            {
                var payload = new { instructions };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PatchAsync($"assistants/{assistantId}", content);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating assistant {e}", e);
                throw;
            }
        }

        public async Task<Assistant> DeleteAssistantAsync(string assistantId)
        {
            try
            {
                using var response = await _httpClient.DeleteAsync($"assistants/{assistantId}");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Assistant>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting assistant {e}", e);
                throw;
            }
        }
        public async Task<AssistantList> ListAssistantsAsync()
        {
            try
            {
                using var response = await _httpClient.GetAsync("assistants");

                // This ensures a successful status code (e.g. 200 OK), or throws an exception.
                var jsonResponse = await EnsureSuccess(response);
                // Configure JsonSerializerSettings to include your custom converter for Tools.
                var settings = new JsonSerializerSettings
                {
                    Converters = new[] { new ToolConverter() }
                };
                // Deserialize JSON to the AssistantList object using the provided settings.
                return JsonConvert.DeserializeObject<AssistantList>(jsonResponse, settings);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while listing assistants: {e}", e);
                throw;
            }
        }

        public async Task<AssistantFile> CreateAssistantFileAsync(string assistantId, string fileId)
        {
            try
            {
                var payload = new { file_id = fileId };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync($"assistants/{assistantId}/files", content);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<AssistantFile>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while Creating  assistant file {e}", e);
                throw;
            }
        }

        public async Task<AssistantFile> RetrieveAssistantFileAsync(string assistantId, string fileId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"assistants/{assistantId}/files/{fileId}");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<AssistantFile>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while  Retrieve Assistant file {e}", e);
                throw;
            }
        }

        public async Task<AssistantFileDeleted> DeleteAssistantFileAsync(string assistantId, string fileId)
        {
            try
            {
                using var response = await _httpClient.DeleteAsync($"assistants/{assistantId}/files/{fileId}");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<AssistantFileDeleted>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while deleting assistant file {e}", e);
                throw;
            }
        }

        public async Task<AssistantList> ListAssistantFilesAsync(string assistantId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"assistants/{assistantId}/files");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<AssistantList>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while listing assistant file {e}", e);
                throw;
            }
        }

        private async Task<string> EnsureSuccess(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Server returned {response.StatusCode}: {errorContent}");
            }
        }

      

    }
 }