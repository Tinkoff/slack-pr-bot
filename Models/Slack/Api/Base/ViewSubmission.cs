using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ViewSubmission
    {
        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("view")]
        public ModalViewSubmission View { get; set; }
    }
}