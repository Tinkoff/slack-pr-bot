using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class Ref
    {
        [JsonProperty("displayId")]
        public string DisplayId { get; set; }

        [JsonProperty("repository")]
        public Repository Repository { get; set; }
    }
}
