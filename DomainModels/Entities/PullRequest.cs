using System;
using System.Collections.Generic;

namespace SlackPrBot.DomainModels.Entities
{
    internal class PullRequest
    {
        public User Author { get; set; }
        public ICollection<PullRequestChannel> ChannelMap { get; set; } = new List<PullRequestChannel>();
        public ICollection<Comment> Comments { get; set; }
        public DateTime Created { get; set; }
        public string FromRef { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime LastActiveDate { get; set; }
        public string ProjectName { get; set; }
        public int PullId { get; set; }
        public int? Iid { get; set; }
        public string RepoName { get; set; }
        public ICollection<Reviewer> Reviewers { get; set; }
        public string TaskId { get; set; }
        public ICollection<PullRequestWatch> Watch { get; set; }
    }
}