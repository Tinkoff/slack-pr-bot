using System;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class Note
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("discussion_id")]
        public string DiscussionId { get; set; }
        [JsonProperty("note")]
        public string NoteText { get; set; }
    }
}
