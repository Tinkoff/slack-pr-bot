using System;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api.Jira
{
    internal class ReadyStatusMove : ISlackMessage
    {
        public string Channel { get; set; }

        public string TaskId { get; set; }

        public int PullId { get; set; }

        public string ProjectName { get; set; }

        public string RepoName { get; set; }

        public bool IsGitlab { get; set; }

        public int? Iid { get; set; }

        public string BaseRepoPath { get; set; }

        public string JiraUrl { get; set; }


        public Message ToSlackMessage()
        {
            return new Message
            {
                Text = $"Would you like to move Task {TaskId} to \"ready to merge\" status?",
                Channel = Channel,
                AsUser = true,
                Blocks = new[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = $"Would you like to move <{JiraUrl}/browse/{TaskId}|:jira: {TaskId}> for <{RepoHelper.PullRequestPath(IsGitlab, BaseRepoPath, ProjectName, RepoName, PullId, Iid)}> to `ready to merge` status?"
                        },
                        Accessory = new ActionElement
                        {
                            Type = ActionElement.ButtonType,
                            Style = ActionElement.StylePrimary,
                            Text = new PlainTextObject
                            {
                                Text = "Move"
                            },
                            ActionId = "jira-move-issue",
                            Value = $"{PullId}"
                        }
                    }
                }
            };
        }
    }
}
