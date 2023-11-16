
using dbc_Dave.Data;
using dbc_Dave.Data.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace dbc_Dave.Services
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly ILogger _logger;
        private readonly IDbContextFactory<dbc_UsersContext> _contextFactory;

        public RedisService(string host, IDbContextFactory<dbc_UsersContext> contextFactory, ILogger<RedisService> logger)
        {
            _redisConnection = ConnectionMultiplexer.Connect(host);
            _logger = logger;
            _contextFactory = contextFactory;

        }

        public async Task<List<DaveMessage>> GetOrCreateMessagesAsync(string key)
        {
            var db = _redisConnection.GetDatabase();

            // Try to obtain the JSON object from Redis by key
            var data = await db.StringGetAsync(key);

            // If the data exist in Redis, deserialize it to List<DaveMessage>
            if (!data.IsNull) { 
                return JsonConvert.DeserializeObject<List<DaveMessage>>(data.ToString());
            }
            else
            {
                // If the data does not exist in Redis, create a new list
                List<DaveMessage> messages = new List<DaveMessage>();
                return messages;
            }
         
        }

        public async Task SetValue(string key, string value, DataQuery? query = null, string? currentUser = null )
        {
            var db = _redisConnection.GetDatabase();
            await db.StringSetAsync(key, value, TimeSpan.FromHours(1));
            using var context = _contextFactory.CreateDbContext();

            if (query != null) { 
                DataQuery currentQuery = context.Queries.Where(x => x.UserId == currentUser && x.QueryName == query.QueryName).FirstOrDefault();
                if(currentQuery != null) {
                    currentQuery.QueryText = query.QueryText;
                    currentQuery.QueryName = query.QueryName;
                    currentQuery.UserId = query.UserId;
                    context.Queries.Update(currentQuery);
                }
                else
                {
                    context.Queries.Add(query);
                }
                
                await context.SaveChangesAsync();
            }

            }

        public async Task<string> GetValue(string key)
        {
            var db = _redisConnection.GetDatabase();
            var response = await db.StringGetAsync(key);
            using var context = _contextFactory.CreateDbContext();

            if (response.IsNullOrEmpty)
            {
                string[] splt = key.Split(':');
                var queryName = splt[0];
                var username = splt[1];

                string? json = context.Queries.Where(q =>  q.QueryName == queryName && q.UserId == username).Select(m => m.QueryText).FirstOrDefault()?.ToString();

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

        public async Task DeleteQuery(string queryName, string currentUser)
        {
            using var context = _contextFactory.CreateDbContext();

            // Get the Query object
            var entry = await context.Queries
                .Where(x => x.UserId == currentUser && x.QueryName == queryName)
                .FirstOrDefaultAsync();
            // If Query exists, delete it
            if (entry != null)
            {
                context.Queries.Remove(entry);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Query not found");
            }

            // Delete from Redis as well if exists
            var key = queryName + ":" + currentUser;
            var db = _redisConnection.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public List<string> GetQueryNames(string currentUser)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Queries.Where(x => x.UserId == currentUser).Select(y => y.QueryName).OrderBy(queryName => queryName).ToList();
        }

        public List<string> GetKeys(string currentUser)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.Queries.Where(x => x.UserId == currentUser).Select(y => y.QueryName).ToList();
        }

    }
}