using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class UserConversationResult : BaseResult
    {
        [JsonProperty("channels")]
        public Channel[] Channels { get; set; }
    }
}
