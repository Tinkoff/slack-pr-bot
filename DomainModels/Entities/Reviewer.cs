using System;

using SlackPrBot.DomainModels.Enum;

namespace SlackPrBot.DomainModels.Entities
{
    internal class Reviewer
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public PullRequest PullRequest { get; set; }
        public ReviewersStatus Status { get; set; }

        public User User { get; set; }
    }
}