using dbc_Dave.Data.Models;

namespace dbc_Dave.Services
{
    public interface IThreadService
    {
        Task<ThreadResponse> CreateThreadAsync();
        Task<ThreadResponse> RetrieveThreadAsync(string threadId);
        Task<ThreadResponse> ModifyThreadAsync(string threadId, ModifyThreadRequest modifyRequest);
        Task<DeleteThreadResponse> DeleteThreadAsync(string threadId);

        Task<Run> ThreadAndRunAsync(ThreadAndRun threadAndRun);

        Task<StepList> ListRunStepsAsync(string run_id, string thread_id);
    }
}
