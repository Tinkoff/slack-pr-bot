using System;

namespace SlackPrBot.Models.Slack.Model
{
    internal class StaleNotificationTask
    {
        public string Id { get; set; }
        public TimeSpan Overdue { get; set; }
        public string ProjectName { get; set; }
        public int PullId { get; set; }
        public string RepoName { get; set; }
        public string TaskId { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }
    }
}
