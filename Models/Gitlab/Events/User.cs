using System;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class User
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("username")]
        public string UserName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
