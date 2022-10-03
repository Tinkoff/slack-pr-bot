using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using SlackPrBot.Services;

namespace SlackPrBot.Controllers
{
    [Route("stash")]
    [ApiController]
    public class StashController : ControllerBase
    {
        private readonly IStashEventService _eventService;

        public StashController(IStashEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [Route("ping")]
        public string Ping()
        {
            return "pong";
        }

        [HttpPost]
        [Route("")]
        public Task StashEventAsync([FromBody] JObject eventData)
        {
            return _eventService.StashEventAsync(eventData);
        }
    }
}
