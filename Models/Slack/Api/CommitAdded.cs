using System.Collections.Generic;
using System.Linq;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class CommitAdded : ISlackMessage
    {
        public string Channel { get; set; }
        public string CommitAuthorName { get; set; }

        public string CommitId { get; set; }
        public string ProjectName { get; set; }
        public int PullId { get; set; }
        public string RepoName { get; set; }
        public string ThreadTs { get; set; }
        public IReadOnlyCollection<DomainModels.Entities.User> WatchUsers { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                ThreadTs = ThreadTs,
                Channel = Channel,
                Text = $"User `{CommitAuthorName}` added new commit",
                Blocks = new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = $":new_stash: {CreateWatchList()}User `{CommitAuthorName}` added new commit"
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
                            new TextObject
                            {
                                Text = $"<{RepoHelper.PullRequestCommitPath(IsGitlab, BaseRepoPath, ProjectName, RepoName, Iid, CommitId)}>"
                            },
                        }
                    }
                }
            };
        }

        private string CreateWatchList()
        {
            var logins = string.Join(", ", WatchUsers.Where(x => x != null).Distinct().Select(x => UserExtensions.FormatUser(x.SlackUserId, x.Login, true)));
            return logins.Length > 0 ? $"{logins}. " : "";
        }
    }
}
