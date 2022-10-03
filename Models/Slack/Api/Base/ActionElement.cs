using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ActionElement
    {
        public static readonly string ButtonType = "button";

        public static readonly string OverflowType = "overflow";

        public static readonly string StyleDanger = "danger";

        public static readonly string StylePrimary = "primary";

        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("options")]
        public SelectOption[] Options { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("text")]
        public PlainTextObject Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}