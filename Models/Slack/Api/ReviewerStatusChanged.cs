using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;
using SlackPrBot.Models.Slack.Api.Enums;

namespace SlackPrBot.Models.Slack.Api
{
    internal class ReviewerStatusChanged : ISlackMessage
    {
        public string AuthorId { get; set; }
        public string Channel { get; set; }
        public string ProjectName { get; set; }
        public int PullId { get; set; }
        public string RepoName { get; set; }
        public string ReviewerName { get; set; }
        public ReviewersStatus Status { get; set; }
        public string ThreadTs { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                Blocks = new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                           Text = SelectText(true)
                        }
                    },
                    new DividerBlock(),
                    new ContextBlock
                    {
                        Elements = new[]
                        {
                            new TextObject
                            {
                                Text = $"<{RepoHelper.PullRequestPath(IsGitlab, BaseRepoPath, ProjectName, RepoName, PullId, Iid)}>"
                            },
                        }
                    }
                },
                Channel = Channel,
                ThreadTs = ThreadTs,
                Text = SelectText(false)
            };
        }

        private string SelectText(bool withIco)
        {
            switch (Status)
            {
                case ReviewersStatus.RemoveApproval:
                    return $"{(withIco ? ":point_right: " : "")}{(string.IsNullOrEmpty(AuthorId) ? "" : $"<@{AuthorId}>. ")}User `{ReviewerName}` remove approval from pull";

                case ReviewersStatus.Approved:
                    return $"{(withIco ? ":thumbsup: " : "")}{(string.IsNullOrEmpty(AuthorId) ? "" : $"<@{AuthorId}>. ")} User `{ReviewerName}` approved pull";

                case ReviewersStatus.Unapproved:
                    return $"{(withIco ? ":thumbsdown: " : "")}{(string.IsNullOrEmpty(AuthorId) ? "" : $"<@{AuthorId}>. ")}User `{ReviewerName}` unapproved pull";
            }

            return null;
        }
    }
}