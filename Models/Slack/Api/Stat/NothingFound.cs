using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api.Stat
{
    internal class NothingFound : ISlackMessage
    {
        public Message ToSlackMessage()
        {
            return new Message
            {
                ResponseType = Message.ResponseTypeEphemeral,
                Text = "Nothing found for this request",
                Blocks = new[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = ":zipper_mouth_face: Nothing found for this request"
                        }
                    }
                }
            };
        }
    }
}
