using System.Collections.Generic;
using System.Linq;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class PullCreated : ISlackMessage
    {
        public string Channel { get; set; }
        public string AuthorName { get; set; }
        public string ProjectName { get; set; }
        public int PullId { get; set; }
        public string RepoName { get; set; }
        public string ThreadTs { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }
        public IReadOnlyCollection<string> NotifyUsers { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                ThreadTs = ThreadTs,
                Channel = Channel,
                Text = $"Pull created by `{AuthorName}`",
                Blocks = new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = $":new_stash: {CreateNotifyList()}Pull created by `{AuthorName}`"
                        }
                    },
                    new DividerBlock(),
                    new ContextBlock
                    {
                        Elements = new []
                        {
                            new TextObject
                            {
                                Text = $"<{RepoHelper.PullRequestPath(IsGitlab, BaseRepoPath, ProjectName, RepoName, PullId, Iid)}>"
                            },
                        }
                    }
                }
            };
        }

        private string CreateNotifyList()
        {
            var logins = string.Join(", ", NotifyUsers.Where(x => x != null).Distinct());
            return logins.Length > 0 ? $"{logins}. " : "";
        }
    }
}
