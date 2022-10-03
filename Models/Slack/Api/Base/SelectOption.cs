using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class SelectOption
    {
        [JsonProperty("text")]
        public PlainTextObject Text { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}