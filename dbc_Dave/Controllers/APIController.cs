using dbc_Dave.Data;
using dbc_Dave.Data.Models;
using dbc_Dave.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing.Text;
using System.Security.Claims;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dbc_Dave.Controllers
{

    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class APIController : ControllerBase
    {
        private IOpenAI _openAI;
        private ILogger<APIController> _logger;
        private IRedisService _redisService;
        private IServiceProvider _serviceProvider;


        public APIController(IOpenAI openAI, IRedisService redisService, ILogger<APIController> logger, IServiceProvider serviceProvider)
        {
            _openAI = openAI;
            _logger = logger;
            _redisService = redisService;
            _serviceProvider = serviceProvider;

        }

        // GET: api/<APIController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            IHeaderDictionary headers = HttpContext.Request.Headers;
            string user = headers["User"].ToString();

            List<string> keys = _redisService.GetKeys(user);
            return keys;
        }

        // GET api/<APIController>/5
        [HttpGet("Details/{id}")]
        public async Task<ChatEntry> Details(string id)
        {
            ChatEntry chatResponse = new ChatEntry();
            CancellationTokenSource token = new CancellationTokenSource();

          
            IHeaderDictionary headers = HttpContext.Request.Headers;
            string user = headers["User"].ToString();

            var v =  await _redisService.GetValue(id + ":" + user);

            var chatEntries = JsonConvert.DeserializeObject<List<CreateMessageRequest>>(v);

            var openaiChatHistory = chatEntries.Select(m => new KeyValuePair<string, string>(m.role, m.content)).ToList();

            var q = await _openAI.ChatCompletionsAsync("gpt-4-1106-preview", openaiChatHistory);

            JObject jsonObject = JObject.Parse(q);

            chatResponse.Content = jsonObject["choices"]?[0]?["message"]?["content"]?.ToString() ?? "";
            chatResponse.Role = jsonObject["choices"]?[0]?["message"]?["role"]?.ToString() ?? "";

            return chatResponse;
        }

        // POST api/<APIController>
        [HttpPost("RunBot/{id}")]
        public async Task<string> RunBot(string id)
        {
            
            IHeaderDictionary headers = HttpContext.Request.Headers;
            string user = headers["User"].ToString();

            var v = await _redisService.GetValue(id + ":" + user);

            return null;
        }


        // PUT api/<APIController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<APIController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class ChatEntry
        {
            public string Role { get; set; }
            public string Content { get; set; }
            public bool EditMode { get; set; }
        }

    }
}
