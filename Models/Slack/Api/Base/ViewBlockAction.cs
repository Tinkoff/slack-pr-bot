using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class ViewBlockAction
    {
        [JsonProperty("actions")]
        public ActionResult[] Actions { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("view")]
        public ModalViewSubmission View { get; set; }
    }
}