using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class OpenModal
    {
        [JsonProperty("trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty("view")]
        public ModalView View { get; set; }
    }
}
