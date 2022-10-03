using System;
using System.Collections.Generic;

namespace SlackPrBot.DomainModels.Entities
{
    internal class Comment
    {
        public User Author { get; set; }
        public int CommentId { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public PullRequest PullRequest { get; set; }
        public ICollection<CommentSlack> SlackMessages { get; set; } = new List<CommentSlack>();
        public string Text { get; set; }
    }
}
