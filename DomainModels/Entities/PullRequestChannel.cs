using System;

namespace SlackPrBot.DomainModels.Entities
{
    internal class PullRequestChannel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime? LastNotificationDate { get; set; }
        public PullRequest PullRequest { get; set; }
        public Settings Settings { get; set; }
        public string SlackTs { get; set; }
    }
}
