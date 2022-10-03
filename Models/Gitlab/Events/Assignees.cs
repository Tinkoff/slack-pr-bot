using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace SlackPrBot.Models.Gitlab.Events
{
    internal class Assignees
    {
        [JsonProperty("previous")]
        public IReadOnlyCollection<User> Previous { get; set; }
        [JsonProperty("current")]
        public IReadOnlyCollection<User> Current { get; set; }
    }
}
