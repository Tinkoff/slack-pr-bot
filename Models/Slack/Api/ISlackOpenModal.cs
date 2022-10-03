using SlackPrBot.Models.Slack.Api.Base;

namespace SlackPrBot.Models.Slack.Api
{
    internal interface ISlackOpenModal
    {
        OpenModal ToSlackModalMessage();
    }
}
