using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class Repository
    {
        [JsonIgnore]
        public string SlugLower => Slug?.ToLower();

        [JsonProperty("project")]
        public Project Project { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }
    }
}