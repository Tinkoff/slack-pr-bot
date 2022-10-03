using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class BaseResult
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("ok")]
        public bool Ok { get; set; }
    }
}
