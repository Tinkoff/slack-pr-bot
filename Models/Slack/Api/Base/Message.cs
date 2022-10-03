using System.Collections.Generic;
using Newtonsoft.Json;

using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.Models.Slack.Api.Base
{
    public class Message
    {
        public static readonly string ResponseTypeEphemeral = "ephemeral";
        public static readonly string ResponseActionErrors = "errors";

        [JsonProperty("blocks")]
        public IBlock[] Blocks { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("delete_original")]
        public bool? DeleteOriginal { get; set; }

        [JsonProperty("replace_original")]
        public bool? ReplaceOriginal { get; set; }

        [JsonProperty("as_user")]
        public bool? AsUser { get; set; }

        [JsonProperty("response_type")]
        public string ResponseType { get; set; }

        [JsonProperty("response_action")]
        public string ResponseAction { get; set; }

        [JsonProperty("errors")]
        public IDictionary<string, string> Errors { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("thread_ts")]
        public string ThreadTs { get; set; }

        [JsonProperty("ts")]
        public string Ts { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }
}