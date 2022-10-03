using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab
{
    internal class MergeRequestResponse
    {
        [JsonProperty("state")]
        public string State { get; set; }
    }
}