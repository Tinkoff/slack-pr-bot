using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base.Blocks
{
    internal class InputBlock : IBlock
    {
        public string BlockId { get; set; }

        [JsonProperty("element")]
        public Element Element { get; set; }

        [JsonProperty("hint")]
        public PlainTextObject Hint { get; set; }

        [JsonProperty("label")]
        public PlainTextObject Label { get; set; }

        [JsonProperty("optional")]
        public bool Optional { get; set; }

        public string Type => "input";
    }
}