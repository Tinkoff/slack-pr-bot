using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class UserGroup
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("handle")]
        public string Handle { get; set; }
    }
}