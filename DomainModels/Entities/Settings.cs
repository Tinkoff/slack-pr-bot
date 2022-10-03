using System;
using System.Linq;

namespace SlackPrBot.DomainModels.Entities
{
    internal class Settings
    {
        public string ChannelId { get; set; }
        public string DevelopmentStatuses { get; set; }
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool JiraSupport { get; set; }
        public string ReadyStatuses { get; set; }
        public string ReadyToReviewStatuses { get; set; }
        public string ReviewStatuses { get; set; }
        public string ClosedStatuses { get; set; }
        public string Project { get; set; }
        public string Repo { get; set; }
        public TimeSpan StaleTime { get; set; }
        public bool IsGitlab { get; set; }
        public string ExcludeToBranches { get; set; }
        public string RepoUrl { get; set; }
        public string GitlabToken { get; set; }
        public string NotifyUsers { get; set; }

        public string[] GetExcludeBranches()
        {
            if (string.IsNullOrEmpty(ExcludeToBranches)) return Array.Empty<string>();

            return ExcludeToBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        }
    }
}
