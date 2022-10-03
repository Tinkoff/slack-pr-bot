using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;
using SlackPrBot.Extensions;

namespace SlackPrBot.Models.Slack.Api.Stat
{
    public class ReadyToMergeStat : ISlackMessage
    {
        public IReadOnlyCollection<PrInfo> Tasks { get; set; }

        public string JiraUrl { get; set; }


        public Message ToSlackMessage()
        {
            return new Message
            {
                ResponseType = Message.ResponseTypeEphemeral,
                Text = "Ready to merge PRs",
                Blocks = GenerateBlocks()
            };
        }

        private string FormatTimespan(TimeSpan timeSpan)
        {
            return string.Format("{0:%d} day(s) {0:%h} hour(s) {0:%m} minute(s) ago", timeSpan);
        }

        private IBlock[] GenerateBlocks()
        {
            var blocks = new List<IBlock>
            {
                new SectionBlock
                {
                    Text = new TextObject
                    {
                        Text = ":chart_with_upwards_trend: Ready to merge PRs"
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
                        Text = $"<{RepoHelper.PullRequestPath(task.IsGitlab, task.BaseRepoPath, task.ProjectName, task.RepoName, task.PullId, task.Iid, true)}>{GenerateTaskLink(task.TaskId)} last action was *{FormatTimespan(task.TimeSinceLastAction)}*"
                    }
                });
            }

            return blocks.ToArray();
        }

        private string GenerateTaskLink(string taskId)
        {
            return string.IsNullOrEmpty(taskId) ? "" : $" for <{JiraUrl}/browse/{taskId}|:jira: *{taskId}*>";
        }
    }
}
