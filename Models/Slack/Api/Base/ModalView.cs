using Newtonsoft.Json;

using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ModalView
    {
        [JsonProperty("blocks")]
        public IBlock[] Blocks { get; set; }

        [JsonProperty("callback_id")]
        public string CallbackId { get; set; }

        [JsonProperty("clear_on_close")]
        public bool ClearOnClose { get; set; }

        [JsonProperty("close")]
        public PlainTextObject Close { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("private_metadata")]
        public string PrivateMetadata { get; set; }

        [JsonProperty("submit")]
        public PlainTextObject Submit { get; set; }

        [JsonProperty("title")]
        public PlainTextObject Title { get; set; }

        [JsonProperty("type", Required = Required.Default)]
        public string Type => "modal";
    }
}