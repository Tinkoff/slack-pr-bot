using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base.Blocks
{
    internal class ActionBlock : IBlock
    {
        public string BlockId { get; set; }

        [JsonProperty("elements")]
        public ActionElement[] Elements { get; set; }

        public string Type => "actions";
    }
}
