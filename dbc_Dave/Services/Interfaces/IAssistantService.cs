using dbc_Dave.Data.Models;

namespace dbc_Dave.Services.Interfaces
{
    public interface IAssistantService
    {
        Task<Assistant> CreateAssistantAsync(string model, string name, string instructions, List<Tool> tools);
        Task<Assistant> RetrieveAssistantAsync(string assistantId);
        Task<Assistant> ModifyAssistantAsync(string assistantId, string instructions);
        Task<Assistant> DeleteAssistantAsync(string assistantId);
        Task<AssistantList> ListAssistantsAsync();
        Task<AssistantFile> CreateAssistantFileAsync(string assistantId, string fileId);
        Task<AssistantFile> RetrieveAssistantFileAsync(string assistantId, string fileId);
        Task<AssistantFileDeleted> DeleteAssistantFileAsync(string assistantId, string fileId);
        Task<AssistantList> ListAssistantFilesAsync(string assistantId);
    }

}
