using System;
using System.Collections.Generic;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;
using SlackPrBot.Models.Slack.Model;

namespace SlackPrBot.Models.Slack.Api
{
    internal class StaleNotification : ISlackMessage
    {
        public string Channel { get; set; }
        public IReadOnlyCollection<StaleNotificationTask> Tasks { get; set; }
        public string JiraUrl { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                Channel = Channel,
                Text = "@here Some PRs overdue",
                Blocks = GenerateBlocks()
            };
        }

        private string FormatOverdue(TimeSpan overdue)
        {
            return string.Format("{0:%d} day(s) {0:%h} hour(s) {0:%m} minute(s)", overdue);
        }

        private IBlock[] GenerateBlocks()
        {
            var blocks = new List<IBlock>
            {
                new SectionBlock
                {
                    Text = new TextObject
                    {
                        Text = ":stopwatch: @here Some PRs overdue"
                    }
                },
                new DividerBlock(),
            };

            foreach (var task in Tasks)
            {
                blocks.Add(new SectionBlock
                {
                    Text = new TextObject
                    {
                        Text = $"<{RepoHelper.PullRequestPath(task.IsGitlab, task.BaseRepoPath, task.ProjectName, task.RepoName, task.PullId, task.Iid, true)}>{GenerateTaskLink(task.TaskId)} overdue by *{FormatOverdue(task.Overdue)}*"
                    }
                });
            }

            return blocks.ToArray();
        }


        private string GenerateTaskLink(string taskId)
        {
            return string.IsNullOrEmpty(taskId) ? "" : $" for <${JiraUrl}/browse/{taskId}|:jira: *{taskId}*>";
        }
    }
}