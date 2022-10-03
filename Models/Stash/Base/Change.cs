using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class Change
    {
        [JsonProperty("ref")]
        public Ref Ref { get; set; }

        [JsonProperty("toHash")]
        public string ToHash { get; set; }
    }
}
