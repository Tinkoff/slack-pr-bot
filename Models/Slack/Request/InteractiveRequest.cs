using Microsoft.AspNetCore.Mvc;

namespace SlackPrBot.Models.Slack.Request
{
    public class InteractiveRequest
    {
        [FromForm(Name = "payload")]
        public string Payload { get; set; }
    }
}
