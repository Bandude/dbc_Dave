
using dbc_Dave.Data.Models;

namespace dbc_Dave.Services
{
    public interface IRedisService
    {
        Task SetValue(string key, string value, DataQuery? query = null, string? currentUser = null);
        Task<string> GetValue(string key);
        List<string> GetKeys(string currentUser);
        Task<List<DaveMessage>> GetOrCreateMessagesAsync(string key);
        Task DeleteQuery(string queryName, string currentUser);
    }
}