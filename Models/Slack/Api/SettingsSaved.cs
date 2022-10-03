using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api
{
    internal class SettingsSaved : ISlackMessage
    {
        private readonly string _channel;
        private readonly string _userId;

        public SettingsSaved(string channel, string userId)
        {
            _channel = channel;
            _userId = userId;
        }

        public Message ToSlackMessage()
        {
            return new Message
            {
                Channel = _channel,
                User = _userId,
                Text = "Settings saved succesfully",
                Blocks = new[]
                {
                    new SectionBlock {
                        Text = new TextObject
                        {
                            Text =  ":thumbsup: Settings saved succesfully"
                        }
                    }
                }
            };
        }
    }
}