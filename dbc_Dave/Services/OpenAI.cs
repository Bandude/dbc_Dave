using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using dbc_Dave.Data.Models;
using Microsoft.SqlServer.Server;


namespace dbc_Dave.Services
{
    public class OpenAI : IOpenAI
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger _logger;


        public OpenAI(string apiKey, ILogger logger)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            _logger = logger;
        }




        public async Task<string> TranscribeAudioAsync(string audioFilePath, string model)
        {
            try
            {
                var requestUri = "audio/transcriptions";
                using var formData = new MultipartFormDataContent();

                var fileContent = new ByteArrayContent(File.ReadAllBytes(audioFilePath));
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
                formData.Add(fileContent, "file", System.IO.Path.GetFileName(audioFilePath));
                formData.Add(new StringContent(model), "model");

                var response = await _httpClient.PostAsync(requestUri, formData);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while transcribing audio. File Path:{filePath}, Model:{model}", audioFilePath, model);
                throw; 
            }
        }

        public async Task<string> ChatCompletionsAsync(string model, List<KeyValuePair<string, string>> messages, CancellationToken cancellationToken = default)
        {
            try
            {
                var requestUri = "chat/completions";
                var formattedMessages = messages.Select(x => new { role = x.Key, content = x.Value }).ToList();
                var content = new
                {
                    model,
                    messages = formattedMessages,
                };

                var jsonContent = JsonConvert.SerializeObject(content);
                using var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUri, stringContent, cancellationToken);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();


            }catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred while generating chat completions. Model:{model}, Messages:{messages}", model, JsonConvert.SerializeObject(messages));
                throw; 
            }

        }

        public async Task<ModelsList> GetModelsAsync()
        {

            var requestUri = "models";
            var response = await _httpClient.GetAsync(requestUri);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(response.Content.ReadAsStringAsync());

            ModelsList modelsList = JsonConvert.DeserializeObject<ModelsList>(await response.Content.ReadAsStringAsync());
            return modelsList;
        }
    }
}