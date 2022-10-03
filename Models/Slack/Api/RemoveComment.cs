using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class RemoveComment : ISlackMessage
    {
        public string AuthorUser { get; set; }
        public string Channel { get; set; }
        public string ThreadTs { get; set; }
        public string Ts { get; set; }

        public Message ToSlackMessage()
        {
            return new Message
            {
                Blocks = new[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                           Text = $":use_threads: User `{AuthorUser}` removed comment"
                        }
                    }
                },
                Channel = Channel,
                Ts = Ts,
                ThreadTs = ThreadTs,
                Text = $"User `{AuthorUser}` removed comment"
            };
        }
    }
}
