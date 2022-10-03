using System;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class MergeRequestEvent
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("object_attributes")]
        public MergeRequest ObjectAttributes { get; set; }
    }
}
