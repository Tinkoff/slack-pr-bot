using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class Channel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("is_channel")]
        public bool IsChannel { get; set; }

        [JsonProperty("is_group")]
        public bool IsGroup { get; set; }

        [JsonProperty("is_private")]
        public bool IsPrivate { get; set; }
    }
}
