using System;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class MergeRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("iid")]
        public int Iid { get; set; }
        [JsonProperty("author_id")]
        public int AuthorId { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("source_branch")]
        public string SourceBranch { get; set; }
        [JsonProperty("target_branch")]
        public string TargetBranch { get; set; }
        [JsonProperty("target")]
        public RepoInfo Target { get; set; }
        [JsonProperty("source")]
        public RepoInfo Source { get; set; }
        [JsonProperty("last_commit")]
        public LastCommit LastCommit { get; set; }
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("oldrev")]
        public string OldRev { get; set; }
    }
}
