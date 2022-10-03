using System;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class LastCommit
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
