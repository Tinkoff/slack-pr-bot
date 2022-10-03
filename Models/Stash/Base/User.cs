using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class User
    {
        public string AlternativeName => Name ?? DisplayName ?? EmailAddress;

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
