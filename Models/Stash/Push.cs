using Newtonsoft.Json;

using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Models.Stash
{
    public class Push
    {
        [JsonProperty("eventKey")]
        public string EventKey { get; set; }

        [JsonProperty("actor")]
        public User Actor { get; set; }

        [JsonProperty("changes")]
        public Change[] Changes { get; set; }

        [JsonProperty("repository")]
        public Repository Repository { get; set; }
    }
}
