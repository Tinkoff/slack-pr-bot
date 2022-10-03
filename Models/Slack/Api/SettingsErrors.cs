using System.Collections.Generic;
using System;
using SlackPrBot.Models.Slack.Api.Base;

namespace SlackPrBot.Models.Slack.Api
{
    internal class SettingsErrors : ISlackMessage
    {
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public SettingsErrors AddError(string key, string value)
        {
            _errors.Add(key, value);
            return this;
        }

        public bool HasErrors()
        {
            return _errors.Count > 0;
        }

        public Message ToSlackMessage()
        {
            return new Message
            {
                ResponseAction = Message.ResponseActionErrors,
                Errors = _errors
            };
        }
    }
}
