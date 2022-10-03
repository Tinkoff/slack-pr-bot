using System.Linq;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class CreatePullRequest : ISlackMessage
    {
        public string AlternativeName { get; set; }
        public string Channel { get; set; }
        public string CreatorUserId { get; set; }
        public string Description { get; set; }
        public string From { get; set; }
        public string Name { get; set; }
        public string ProjectName { get; set; }
        public int PullId { get; set; }
        public string RepoName { get; set; }
        public string TaskId { get; set; }
        public string To { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }
        public string JiraUrl { get; set; }

        private bool HasMergeConflict => !string.IsNullOrEmpty(Description) && Description.Contains("There was a merge conflict");

        public Message ToSlackMessage()
        {
            return new Message
            {
                Channel = Channel,
                Text = $"User {UserExtensions.FormatUser(CreatorUserId, AlternativeName, HasMergeConflict)} created pull request in `{ProjectName}/{RepoName}`",
                Blocks = new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = $":pullrequest: User {UserExtensions.FormatUser(CreatorUserId, AlternativeName, HasMergeConflict)} created pull request in `{ProjectName}/{RepoName}`"
                        }
                    },
                    new SectionBlock
                    {
                        BlockId = "fromTo",
                        Text = new TextObject
                        {
                            Text = $"`{From}` :arrow_right: `{To}`"
                        }
                    },
                    new DividerBlock(),
                     new SectionBlock
                    {
                        BlockId = "approved",
                        Text = new TextObject
                        {
                            Text = ":thumbsup: *Approved*\nno users"
                        }
                    },
                      new SectionBlock
                    {
                        BlockId = "unapproved",
                        Text = new TextObject
                        {
                            Text = ":thumbsdown: *Unapproved*\nno users"
                        }
                    },
                    new DividerBlock(),
                    new SectionBlock
                    {
                        BlockId = "name",
                        Text = new TextObject
                        {
                            Text = $"*Name*\n{(string.IsNullOrEmpty(Name) ? "no name" : Name)}"
                        }
                    },
                    new SectionBlock
                    {
                        BlockId = "description",
                        Text = new TextObject
                        {
                            Text = $"*Description*\n{(string.IsNullOrEmpty(Description) ? "no description" : Description)}"
                        }
                    },
                    new DividerBlock(),
                    new ContextBlock
                    {
                        BlockId = "context",
                        Elements = new[]
                        {
                            new TextObject
                            {
                                Text = ":hand: waiting"
                            },
                            new TextObject
                            {
                                Text = $"<{RepoHelper.PullRequestPath(IsGitlab, BaseRepoPath, ProjectName, RepoName, PullId, Iid)}>"
                            },
                            !string.IsNullOrEmpty(TaskId) ? new TextObject
                            {
                                Text = $"<${JiraUrl}/browse/{TaskId}|:jira: {TaskId}>"
                            } : null
                        }.Where(x=> x != null).ToArray()
                    }
                }
            };
        }
    }
}
