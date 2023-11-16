

using dbc_Dave.Data.Models;

namespace dbc_Dave.Services.Interfaces
{
    public interface IMessageService
    {

        Task<Message> CreateMessageAsync(string threadId, CreateMessageRequest createMessageRequest);
        Task<Message> RetrieveMessageAsync(string threadId, string messageId);
        //Task<Message> ModifyMessageAsync(string threadId, string messageId, ModifyMessageRequest modifyRequest);
        //Task<MessageListResponse> ListMessagesAsync(string threadId);

    }
}
