using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace SlackPrBot.Services
{
    public interface IGitlabEventService
    {
        Task GitlabEventAsync(JObject eventData);
    }
}