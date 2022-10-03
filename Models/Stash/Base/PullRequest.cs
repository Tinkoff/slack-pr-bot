using Newtonsoft.Json;

namespace SlackPrBot.Models.Stash.Base
{
    public class PullRequest
    {
        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("fromRef")]
        public Ref FromRef { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("iid")]
        public int? Iid { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("toRef")]
        public Ref ToRef { get; set; }
    }
}