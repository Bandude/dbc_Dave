using dbc_Dave.Areas.Identity.Data;
using dbc_Dave.Data;
using dbc_Dave.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using static dbc_Dave.Pages.Index;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace dbc_Dave.Services
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly UserManager<User> _userManager;
        private  dbc_UsersContext _contextdb;

        private readonly ILogger _logger;



        public RedisService(dbc_UsersContext context)
        {
            _redisConnection = ConnectionMultiplexer.Connect("redis:6379");
            _contextdb = context;
        }

        public List<CustomMessage> GetOrCreateMessages(string key)
        {
            var db = _redisConnection.GetDatabase();


            // Try to obtain the JSON object from Redis by key
            var data = db.StringGetAsync(key);

            // If the data exist in Redis, deserialize it to List<CustomMessage>
            if (data.Result.HasValue)
            {
                return JsonConvert.DeserializeObject<List<CustomMessage>>(data.Result.ToString());
            }

            // If the data does not exist in Redis, create a new list
            List<CustomMessage> messages = new List<CustomMessage>();
            return messages;
        }

        public async Task SetValue(string key, string value, DataQuery? query = null, string? currentUser = null )
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value);
            
            if(query != null) { 
                DataQuery currentQuery = _contextdb.Queries.Where(x => x.UserId == currentUser && x.QueryName == query.QueryName).FirstOrDefault();
                if(currentQuery != null) {
                    currentQuery.QueryText = query.QueryText;
                    currentQuery.QueryName = query.QueryName;
                    currentQuery.UserId = query.UserId;
                    _contextdb.Queries.Update(currentQuery);
                }
                else
                {
                    _contextdb.Queries.Add(query);
                }
                
                await _contextdb.SaveChangesAsync();
            }

            }

        public async Task<string> GetValue(string key)
        {
            var db = _redisConnection.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task DeleteKey(string key)
        {
            var db = _redisConnection.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public List<string> GetQueryNames(string currentUser)
        {
            
            return _contextdb.Queries.Where(x => x.UserId == currentUser).Select(y => y.QueryName).ToList();
        }

        public List<string> GetKeys(string currentUser)
        {
            return _contextdb.Queries.Where(x => x.UserId == currentUser).Select(y => y.QueryName).ToList();
        }

    }
}