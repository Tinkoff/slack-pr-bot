using System.Threading.Tasks;
using SlackPrBot.Models.Slack.Request;

namespace SlackPrBot.Services
{
    public interface ISlackSlashService
    {
        Task ProcessSlashAsync(SlackSlashRequest request);
    }
}
