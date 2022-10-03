using System;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class NoteEvent
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("object_attributes")]
        public Note ObjectAttributes { get; set; }
        [JsonProperty("merge_request")]
        public MergeRequest MergeRequest { get; set; }
    }
}
