using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ActionResult
    {
        [JsonProperty("action_id")]
        public string ActionId { get; set; }

        [JsonProperty("block_id")]
        public string BlockId { get; set; }

        [JsonProperty("selected_option")]
        public SelectOption SelectedOption { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}