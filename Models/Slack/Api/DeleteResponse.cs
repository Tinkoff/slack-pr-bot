using SlackPrBot.Models.Slack.Api.Base;

namespace SlackPrBot.Models.Slack.Api
{
    internal class DeleteResponse : ISlackMessage
    {
        public Message ToSlackMessage()
        {
            return new Message
            {
                DeleteOriginal = true
            };
        }
    }
}