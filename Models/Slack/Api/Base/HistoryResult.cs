using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class HistoryResult : BaseResult
    {
        [JsonProperty("messages")]
        public Message[] Messages { get; set; }
    }
}
