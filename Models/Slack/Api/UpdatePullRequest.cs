using System.Linq;
using System.Text.RegularExpressions;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;
using SlackPrBot.Models.Slack.Api.Enums;

namespace SlackPrBot.Models.Slack.Api
{
    internal class UpdatePullRequest : ISlackMessage
    {
        private readonly Message _editMessage;
        private readonly string _jiraUrl;

        public UpdatePullRequest(Message editMessage, string channel, string jiraUrl)
        {
            _editMessage = editMessage;
            _editMessage.Channel = channel;
            _jiraUrl = jiraUrl;
        }

        public UpdatePullRequest ChangeDescription(string description)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == "description") as SectionBlock;
            var element = block.Text;

            element.Text = $"*Description*\n{(string.IsNullOrEmpty(description) ? "no description" : description)}";

            return this;
        }

        public UpdatePullRequest ChangeName(string name)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == "name") as SectionBlock;
            var element = block.Text;

            element.Text = $"*Name*\n{(string.IsNullOrEmpty(name) ? "no name" : name)}";

            return this;
        }

        public UpdatePullRequest SetStatus(PullStatus status)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == "context") as ContextBlock;
            var element = block.Elements.First();
            switch (status)
            {
                case PullStatus.Waiting:
                    element.Text = ":hand: waiting";
                    break;

                case PullStatus.Merged:
                    element.Text = ":thumbsup: merged";
                    break;

                case PullStatus.Deleted:
                    element.Text = ":wastebasket: deleted";
                    break;

                case PullStatus.Declined:
                    element.Text = ":thumbsdown: declined";
                    break;
            }

            return this;
        }

        public Message ToSlackMessage()
        {
            return _editMessage;
        }

        public UpdatePullRequest UpdateFromTo(string from, string to)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == "fromTo") as SectionBlock;
            var element = block.Text;

            element.Text = $"`{from}` :arrow_right: `{to}`";

            return this;
        }

        public UpdatePullRequest UpdateTaskId(string taskId)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == "context") as ContextBlock;

            var element = block.Elements.Length == 3 ? block.Elements.Last() : new TextObject { };

            if (string.IsNullOrEmpty(taskId))
            {
                block.Elements = block.Elements.Take(2).ToArray();
            }
            else
            {
                element.Text = $"<{_jiraUrl}/browse/{taskId}|:jira: {taskId}>";
                if (block.Elements.Length < 3)
                {
                    var elements = block.Elements.ToList();
                    elements.Add(element);
                    block.Elements = elements.ToArray();
                }
            }

            return this;
        }

        public UpdatePullRequest UpdateUser(ReviewersStatus status, string userId, string login)
        {
            if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(login))
            {
                return this;
            }

            switch (status)
            {
                case ReviewersStatus.Approved:
                    RemoveUserFromStatus(ReviewersStatus.Unapproved, userId, login);
                    AddUser(ReviewersStatus.Approved, userId, login);
                    break;

                case ReviewersStatus.Unapproved:
                    RemoveUserFromStatus(ReviewersStatus.Approved, userId, login);
                    AddUser(ReviewersStatus.Unapproved, userId, login);
                    break;

                case ReviewersStatus.RemoveApproval:
                    RemoveUserFromStatus(ReviewersStatus.Approved, userId, login);
                    RemoveUserFromStatus(ReviewersStatus.Unapproved, userId, login);
                    break;
            }

            return this;
        }

        private void AddUser(ReviewersStatus status, string userId, string login)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == (status == ReviewersStatus.Approved ? "approved" : "unapproved")) as SectionBlock;
            var element = block.Text;

            var tag = UserExtensions.FormatUser(userId, login, true);

            if (element.Text.Contains("no users"))
            {
                element.Text = element.Text.Replace("no users", tag);
            }
            else
            {
                element.Text += $", {tag}";
            }
        }

        private void RemoveUserFromStatus(ReviewersStatus status, string userId, string login)
        {
            var block = _editMessage.Blocks.First(x => x.BlockId == (status == ReviewersStatus.Approved ? "approved" : "unapproved")) as SectionBlock;
            var element = block.Text;

            element.Text = Regex.Replace(element.Text, $" ?<@{userId}>,?", "");
            element.Text = Regex.Replace(element.Text, $" ?{login},?", "");
            if (string.IsNullOrEmpty(element.Text?.Trim()) && !element.Text.Contains("no users"))
            {
                element.Text = "no users";
            }
        }
    }
}