using System;

namespace SlackPrBot.Models.Slack.Api.Stat
{
    public class PrInfo
    {
        public int PullId { get; set; }
        public string TaskId { get; set; }
        public TimeSpan TimeSinceLastAction { get; set; }
        public string ProjectName { get; set; }
        public string RepoName { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }
    }
}
