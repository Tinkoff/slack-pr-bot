using System;

namespace SlackPrBot.DomainModels.Entities
{
    internal class PullRequestWatch
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public PullRequest PullRequest { get; set; }
        public User User { get; set; }
    }
}
