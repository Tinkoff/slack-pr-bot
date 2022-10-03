using System;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api.Jira
{
    internal class LogWork : ISlackMessage
    {
        public string TaskId { get; set; }
        public string Channel { get; set; }
        public string JiraId { get; set; }
        public int PullId { get; set; }
        public string Project { get; set; }
        public string Repo { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }
        public string JiraUrl { get; set; }
        public int? Iid { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                Text = $"Log work for task {TaskId}",
                Channel = Channel,
                AsUser = true,
                Blocks = new[]
               {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = $":stopwatch: Log work for task <{JiraUrl}/secure/WorklogCreate!default.jspa?id={JiraId}|:jira: {TaskId}> for <{RepoHelper.PullRequestPath(IsGitlab, BaseRepoPath, Project, Repo, PullId, Iid)}>"
                        }
                    }
                }
            };
        }
    }
}
