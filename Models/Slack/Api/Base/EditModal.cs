using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class EditModal
    {
        [JsonProperty("view")]
        public ModalView View { get; set; }

        [JsonProperty("view_id")]
        public string ViewId { get; set; }
    }
}
