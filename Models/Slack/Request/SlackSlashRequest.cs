using Microsoft.AspNetCore.Mvc;

namespace SlackPrBot.Models.Slack.Request
{
    public class SlackSlashRequest
    {
        [FromForm(Name = "channel_id")]
        public string ChannelId { get; set; }

        [FromForm(Name = "command")]
        public string Command { get; set; }

        [FromForm(Name = "response_url")]
        public string ResponseUrl { get; set; }

        [FromForm(Name = "text")]
        public string Text { get; set; }

        [FromForm(Name = "trigger_id")]
        public string TriggerId { get; set; }

        [FromForm(Name = "user_id")]
        public string UserId { get; set; }
    }
}