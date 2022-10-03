using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class PlainTextObject
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type => "plain_text";
    }
}