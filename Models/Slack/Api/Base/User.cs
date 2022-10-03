using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class User
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
