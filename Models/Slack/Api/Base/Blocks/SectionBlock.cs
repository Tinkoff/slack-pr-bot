using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base.Blocks
{
    internal class SectionBlock : IBlock
    {
        [JsonProperty("accessory")]
        public ActionElement Accessory { get; set; }

        public string BlockId { get; set; }

        [JsonProperty("text")]
        public TextObject Text { get; set; }

        public string Type => "section";
    }
}