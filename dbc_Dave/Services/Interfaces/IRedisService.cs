using dbc_Dave.Data;
using dbc_Dave.Data.Models;
using dbc_Dave.Migrations;
using System.Runtime.CompilerServices;
using static dbc_Dave.Pages.Index;

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