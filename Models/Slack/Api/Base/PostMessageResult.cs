using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class PostMessageResult : BaseResult
    {
        [JsonProperty("ts")]
        public string Ts { get; set; }
    }
}
