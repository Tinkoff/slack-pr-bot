using Newtonsoft.Json;

using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Models.Stash
{
    public class CommentRequest
    {
        [JsonProperty("eventKey")]
        public string EventKey { get; set; }

        [JsonProperty("actor")]
        public User Actor { get; set; }

        [JsonProperty("comment")]
        public Comment Comment { get; set; }

        [JsonProperty("commentParentId")]
        public int? CommentParentId { get; set; }

        [JsonProperty("pullRequest")]
        public PullRequest PullRequest { get; set; }
    }
}
