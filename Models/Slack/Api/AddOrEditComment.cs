using System.Linq;
using System.Text;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class AddOrEditComment : ISlackMessage
    {
        public string AuthorUserName { get; set; }
        public string Channel { get; set; }

        public int CommentId { get; set; }
        public string Message { get; set; }
        public string ProjectName { get; set; }
        public string RepoName { get; set; }
        public int PullId { get; set; }
        public string Quote { get; set; }
        public DomainModels.Entities.User QuoteAuthor { get; set; }
        public string ThreadTs { get; set; }
        public int? Iid { get; set; }
        public bool IsGitlab { get; set; }
        public string BaseRepoPath { get; set; }

        public string Ts { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                Channel = Channel,
                ThreadTs = ThreadTs,
                Ts = Ts,
                Text = GenerateTitle(true),
                Blocks = new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = GenerateTitle(false)
                        }
                    },
                    new DividerBlock(),
                    !string.IsNullOrEmpty(Quote) ? new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = GenerateQuote()
                        }
                    } : null,
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = Message
                        }
                    },
                    new DividerBlock(),
                    new ContextBlock
                    {
                        Elements = new []
                        {
                            new TextObject
                            {
                                Text = $"<{RepoHelper.PullRequestCommentPath(IsGitlab, BaseRepoPath, ProjectName, RepoName, PullId, Iid, CommentId)}>"
                            }
                        }
                    }
                }.Where(x => x != null).ToArray(),
            };
        }

        private string GenerateQuote()
        {
            var result = new StringBuilder();
            if (QuoteAuthor != null)
            {
                result.Append(">>> *Author* ");
                if (!string.IsNullOrEmpty(QuoteAuthor.SlackUserId))
                {
                    result.Append("<@").Append(QuoteAuthor.SlackUserId).Append(">\n");
                }
                else
                {
                    result.Append('`').Append(QuoteAuthor.Login).Append('`');
                }
            }

            if (result.Length == 0)
            {
                result.Append(">>> ");
            }

            result.Append(Quote.Length > 300 ? Quote.Substring(0, 299) + "..." : Quote);

            return result.ToString();
        }

        private string GenerateTitle(bool noIcon)
        {
            return $"{(noIcon ? "" : ":use_threads: ")}User `{AuthorUserName}` {(string.IsNullOrEmpty(Ts) ? "added" : "edited")} comment";
        }
    }
}