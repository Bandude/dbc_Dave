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
        private  dbc_UsersContext _contextdb;
        private readonly ILogger _logger;



        public RedisService(string host, dbc_UsersContext context, ILogger<RedisService> logger)
        {
            _redisConnection = ConnectionMultiplexer.Connect(host);
            _logger = logger;
            _contextdb = context;
        }

        public async Task<List<CustomMessage>> GetOrCreateMessagesAsync(string key)
        {
            var db = _redisConnection.GetDatabase();

            // Try to obtain the JSON object from Redis by key
            var data = await db.StringGetAsync(key);

            // If the data exist in Redis, deserialize it to List<CustomMessage>
            if (!data.IsNull) { 
                return JsonConvert.DeserializeObject<List<CustomMessage>>(data.ToString());
            }
            else
            {
                // If the data does not exist in Redis, create a new list
                List<CustomMessage> messages = new List<CustomMessage>();
                return messages;
            }
         
        }

        public async Task SetValue(string key, string value, DataQuery? query = null, string? currentUser = null )
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value, TimeSpan.FromHours(1));
            
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
            var response = await db.StringGetAsync(key);
            if (response.IsNullOrEmpty)
            {
                string[] splt = key.Split(':');
                var queryName = splt[0];
                var username = splt[1];

                string? json = _contextdb.Queries.Where(q =>  q.QueryName == queryName && q.UserId == username).Select(m => m.QueryText).FirstOrDefault()?.ToString();

                if (json != null)
                {
                    await SetValue(key, json); //save to cache if not there
                    return await db.StringGetAsync(key);
                }
                else
                {
                    throw new Exception("Redis | GetValue: Error");
                }
            }
            else
            {
                return await db.StringGetAsync(key);
            }
            
        }

        public async Task DeleteKey(string key)
        {
            var db = _redisConnection.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public List<string> GetQueryNames(string currentUser)
        {

            return _contextdb.Queries.Where(x => x.UserId == currentUser).Select(y => y.QueryName).OrderBy(queryName => queryName).ToList();
        }

        public List<string> GetKeys(string currentUser)
        {
            return _contextdb.Queries.Where(x => x.UserId == currentUser).Select(y => y.QueryName).ToList();
        }

    }
}