

using dbc_Dave.Data.Models;

namespace dbc_Dave.Services.Interfaces
{
    public interface IMessageService
    {

        Task<MessageResponse> CreateMessageAsync(string threadId, CreateMessageRequest createMessageRequest);
        Task<MessageResponse> RetrieveMessageAsync(string threadId, string messageId);
        Task<MessageResponse> ModifyMessageAsync(string threadId, string messageId, ModifyMessageRequest modifyRequest);
        Task<MessageListResponse> ListMessagesAsync(string threadId);

    }
}
