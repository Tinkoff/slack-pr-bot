using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class UserResult : BaseResult
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
