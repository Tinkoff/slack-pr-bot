using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class Comment
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
