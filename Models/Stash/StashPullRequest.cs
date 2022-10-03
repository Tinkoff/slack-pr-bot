using Newtonsoft.Json;

using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Models.Stash
{
    public class StashPullRequest
    {
        [JsonProperty("eventKey")]
        public string EventKey { get; set; }

        [JsonProperty("actor")]
        public User Actor { get; set; }

        [JsonProperty("pullRequest")]
        public PullRequest PullRequest { get; set; }
    }
}
