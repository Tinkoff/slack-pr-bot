using System.Threading.Tasks;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Request;

namespace SlackPrBot.Services
{
    public interface ISlackInteractiveService
    {
        Task<Message> ProcessInteractiveAsync(InteractiveRequest request);
    }
}
