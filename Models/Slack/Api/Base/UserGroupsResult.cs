using Newtonsoft.Json;

namespace SlackPrBot.Models.Slack.Api.Base
{
    internal class UserGroupsResult : BaseResult
    {
        [JsonProperty("usergroups")]
        public UserGroup[] UserGroups { get; set; }
    }
}