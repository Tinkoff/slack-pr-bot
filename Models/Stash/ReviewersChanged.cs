using Newtonsoft.Json;

using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Models.Stash
{
    public class ReviewersChanged
    {
        [JsonProperty("actor")]
        public User Actor { get; set; }

        [JsonProperty("addedReviewers")]
        public User[] AddedReviewers { get; set; }

        [JsonProperty("pullRequest")]
        public PullRequest PullRequest { get; set; }

        [JsonProperty("removedReviewers")]
        public User[] RemovedReviewers { get; set; }
    }
}