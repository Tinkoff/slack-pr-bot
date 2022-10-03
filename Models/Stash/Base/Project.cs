using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class Project
    {
        [JsonIgnore]
        public string KeyLower => Key?.ToLower();

        [JsonProperty("key")]
        public string Key { get; set; }
    }
}