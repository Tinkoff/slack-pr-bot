using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class TextObject
    {
        public static string MrkDwnType = "mrkdwn";
        public static string PlainTextType = "plain_text";

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; } = MrkDwnType;
    }
}