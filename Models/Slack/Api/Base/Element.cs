using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class Element
    {
        public static readonly string SelectInput = "static_select";
        public static readonly string TextInput = "plain_text_input";

        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("initial_option")]
        public SelectOption InitialOption { get; set; }

        [JsonProperty("initial_value")]
        public string InitialValue { get; set; }

        [JsonProperty("multiline")]
        public bool Multiline { get; set; }

        [JsonProperty("options")]
        public SelectOption[] Options { get; set; }

        [JsonProperty("placeholder")]
        public PlainTextObject Placeholder { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}