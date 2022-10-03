using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class BotNotInChannel : ISlackMessage
    {
        public Message ToSlackMessage()
        {
            return new Message
            {
                ResponseType = Message.ResponseTypeEphemeral,
                Text = "I am not in this channel, please invite me",
                Blocks = new[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = ":zipper_mouth_face: I am not in this channel, please invite me"
                        }
                    }
                }
            };
        }
    }
}
