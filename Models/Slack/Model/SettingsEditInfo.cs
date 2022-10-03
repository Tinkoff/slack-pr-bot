namespace SlackPrBot.Models.Slack.Model
{
    internal class SettingsEditInfo
    {
        public string Id { get; set; }
        public string Project { get; set; }
        public string Repo { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }
    }
}