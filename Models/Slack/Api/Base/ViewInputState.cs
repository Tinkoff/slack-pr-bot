using System.Collections.Generic;
using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ViewInputState
    {
        [JsonProperty("values")]
        public Dictionary<string, Dictionary<string, ValueInputResult>> Values { get; set; }
    }
}
