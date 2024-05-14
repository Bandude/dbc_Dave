using dbc_Dave.Data.Models;

namespace dbc_Dave.Services
{
    public interface IOpenAI
    {
        Task<string> TranscribeAudioAsync(string audioFilePath, string model);
        Task<string> ChatCompletionsAsync(string model, List<KeyValuePair<string, string>> messages, CancellationToken cancellationToken = default);

        Task<ModelsList> GetModelsAsync();
    }
}