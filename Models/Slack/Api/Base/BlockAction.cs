using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class BlockAction
    {
        [JsonProperty("actions")]
        public ActionResult[] Actions { get; set; }

        [JsonProperty("channel")]
        public Channel Channel { get; set; }

        [JsonProperty("response_url")]
        public string ResponseUrl { get; set; }

        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}