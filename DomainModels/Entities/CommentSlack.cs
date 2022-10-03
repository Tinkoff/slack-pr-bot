using System;

namespace SlackPrBot.DomainModels.Entities
{
    internal class CommentSlack
    {
        public Comment Comment { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public PullRequestChannel PullRequestChannel { get; set; }
        public string SlackTs { get; set; }
    }
}
