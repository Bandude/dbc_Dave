using dbc_Dave.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace dbc_Dave.Services
{
    public class AssistantRunsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger _logger;

        public AssistantRunsService(string apiKey, ILogger logger)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/")
            };
            _apiKey = apiKey ?? throw new ArgumentException("API key cannot be null or empty.", nameof(apiKey));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _logger = logger ?? throw new ArgumentException("Logger cannot be null.", nameof(logger));

            _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
        }

        public async Task<string> EnsureSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error occurred during HTTP request: {StatusCode} {Message}", response.StatusCode.ToString(), errorMessage.ToString());
                throw new HttpRequestException($"Request failed with status {response.StatusCode}: {errorMessage}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<Run> CreateRunAsync(string assistantId, string threadId)
        {
            try
            {
                var payload = new { assistant_id = assistantId };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync($"threads/{threadId}/runs", content);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Run>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occurred while creating a run for Assistant ID {assistantId} and Thread ID {threadId}: {e}");
                throw;
            }
        }
        public async Task<Run> RetrieveRunAsync(string threadId, string runId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"threads/{threadId}/runs/{runId}");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Run>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving a run: {Exception}", e);
                throw;
            }
        }

        public async Task<Run> ModifyRunAsync(string threadId, string runId, Dictionary<string, object> metadata)
        {
            try
            {
                var payload = new { metadata };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync($"threads/{threadId}/runs/{runId}", content);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Run>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while modifying a run: {Exception}", e);
                throw;
            }
        }

        public async Task<List<Run>> ListRunsAsync(string threadId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"threads/{threadId}/runs");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<List<Run>>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while listing runs: {Exception}", e);
                throw;
            }
        }

        //public async Task<Run> SubmitToolOutputsToRunAsync(string threadId, string runId, List<ToolOutput> toolOutputs)
        //{
        //    try
        //    {
        //        var payload = new { tool_outputs = toolOutputs };
        //        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

        //        using var response = await _httpClient.PostAsync($"threads/{threadId}/runs/{runId}/submit_tool_outputs", content);
        //        var jsonResponse = await EnsureSuccess(response);

        //        return JsonConvert.DeserializeObject<Run>(jsonResponse);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "Error occurred while submitting tool outputs to a run: {Exception}", e);
        //        throw;
        //    }
        //}

        public async Task<Run> CancelRunAsync(string threadId, string runId)
        {
            try
            {
                using var response = await _httpClient.PostAsync($"threads/{threadId}/runs/{runId}/cancel", null);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Run>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while canceling a run: {Exception}", e);
                throw;
            }
        }

        public async Task<Run> CreateThreadAndRunAsync(string assistantId, List<Microsoft.DotNet.Scaffolding.Shared.Messaging.Message> messages)
        {
            try
            {
                var payload = new
                {
                    assistant_id = assistantId,
                    thread = new { messages }
                };
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using var response = await _httpClient.PostAsync("threads/runs", content);
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<Run>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while creating thread and run: {Exception}", e);
                throw;
            }
        }

        public async Task<RunStep> RetrieveRunStepAsync(string threadId, string runId, string stepId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"threads/{threadId}/runs/{runId}/steps/{stepId}");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<RunStep>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while retrieving a run step: {Exception}", e);
                throw;
            }
        }

        public async Task<List<RunStep>> ListRunStepsAsync(string threadId, string runId)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"threads/{threadId}/runs/{runId}/steps");
                var jsonResponse = await EnsureSuccess(response);

                return JsonConvert.DeserializeObject<List<RunStep>>(jsonResponse);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred while listing run steps: {Exception}", e);
                throw;
            }
        }


    }
}