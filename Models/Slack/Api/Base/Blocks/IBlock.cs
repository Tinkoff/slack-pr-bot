using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base.Blocks
{
    public interface IBlock
    {
        [JsonProperty("block_id")]
        string BlockId { get; set; }

        [JsonProperty("type", Required = Required.Default)]
        string Type { get; }
    }
}
