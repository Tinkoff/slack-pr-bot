using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ValueInputResult
    {
        [JsonProperty("selected_option")]
        public SelectOption SelectedOption { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
