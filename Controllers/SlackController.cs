using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SlackPrBot.Models.Slack.Request;
using SlackPrBot.Services;

namespace SlackPrBot.Controllers
{
    [Route("slack")]
    [ApiController]
    public class SlackController : ControllerBase
    {
        private readonly ISlackInteractiveService _slackInteractiveService;
        private readonly ISlackSlashService _slackSlashService;

        public SlackController(ISlackSlashService slackSlashService, ISlackInteractiveService slackInteractiveService)
        {
            _slackSlashService = slackSlashService;
            _slackInteractiveService = slackInteractiveService;
        }

        [HttpPost]
        [Route("interactive")]
        public async Task<IActionResult> InteractiveAsync([FromForm] InteractiveRequest request)
        {
            var message = await _slackInteractiveService.ProcessInteractiveAsync(request);
            if (message != null)
            {
                var json = JObject.FromObject(message);

                return Content(json.ToString(), "application/json");
            }

            return Ok();

        }

        [HttpPost]
        [Route("slash")]
        public OkResult Slash([FromForm]SlackSlashRequest request)
        {
            try
            {
                return Ok();
            }
            finally
            {
                Response.OnCompleted(async () => await _slackSlashService.ProcessSlashAsync(request));
            }
        }
    }
}