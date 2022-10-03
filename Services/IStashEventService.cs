using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace SlackPrBot.Services
{
    public interface IStashEventService
    {
        Task StashEventAsync(JObject eventData);
    }
}
