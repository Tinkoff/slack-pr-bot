using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base.Blocks
{
    internal class ContextBlock : IBlock
    {
        public string BlockId { get; set; }

        [JsonProperty("elements")]
        public TextObject[] Elements { get; set; }

        public string Type => "context";
    }
}
