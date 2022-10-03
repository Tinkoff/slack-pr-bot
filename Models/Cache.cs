using System;

using SlackPrBot.Models.Slack.Api.Base;

namespace SlackPrBot.Models
{
    internal class Cache
    {
        public DateTime LastUserGroupCheck { get; set; } = DateTime.MinValue;
        public UserGroup[] SlackUserGroups { get; set; }
    }
}