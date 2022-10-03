using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using SlackPrBot.Services;

namespace SlackPrBot.Controllers
{
    [Route("gitlab")]
    [ApiController]
    public class GitlabController : ControllerBase
    {
        private readonly IGitlabEventService _eventService;

        public GitlabController(IGitlabEventService eventService)
        {
            _eventService = eventService;
        }


        [HttpPost]
        [Route("")]
        public Task GitlabEventAsync([FromBody] JObject eventData)
        {
            return _eventService.GitlabEventAsync(eventData);
        }
    }
}
