using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class WrongSlashCommand : ISlackMessage
    {
        public Message ToSlackMessage()
        {
            return new Message
            {
                Text = "Command not recognized",
                ResponseType = Message.ResponseTypeEphemeral,
                Blocks = new IBlock[]
                {
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = ":thinking_face: Command not recognized"
                        }
                    },
                    new DividerBlock(),
                    new SectionBlock
                    {
                        Text = new TextObject
                        {
                            Text = ":gear: use *setup* command for configuration in this channel. Optional parameter `[add]` for adding new integration with repo. If there is no integrations, `add` will be called automatically\n" +
                            ":chart_with_upwards_trend: use *stat* command for checking pr statistics. Required parameter `[open]` for checking all open prs in channel. Optional param `[my]` for all your open prs in channel. You could use `stat open` in im with bot, for checking all your open prs in all channels. You could use `stat rtm` or `stat rtm my` command to get all ready to merge requests."
                        }
                    }
                }
            };
        }
    }
}