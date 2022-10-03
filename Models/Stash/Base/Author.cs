using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class Author
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
