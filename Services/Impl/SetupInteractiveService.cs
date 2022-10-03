using Microsoft.EntityFrameworkCore;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.Base.Blocks;
using SlackPrBot.Models.Slack.Api.ModalViewBuilders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlackPrBot.Services.Impl
{
    internal class SetupInteractiveService
    {
        private readonly Context _context;
        private readonly SlackApi _slackApi;

        public SetupInteractiveService(Context context, SlackApi slackApi)
        {
            _context = context;
            _slackApi = slackApi;
        }

        public Task ProcessBlockActionAsync(BlockAction result)
        {
            var action = result.Actions.FirstOrDefault(x => x.ActionId.StartsWith("settings"));
            if (action != null)
            {
                switch (action.ActionId)
                {
                    case "settings-list-settings":
                        return ProcessListSettingsActionsAsync(result, action.SelectedOption.Value);
                }
            }

            return Task.CompletedTask;
        }

        public async Task<Message> ProcessSubmitAsync(ViewSubmission result)
        {
            var split = result.View.PrivateMetadata.Split('|');
            var channel = split[0];
            var id = split.Length > 1 ? split[1] : null;

            var errors = await ValidateSubmitFormAsync(result, channel, id);
            if (errors != null)
            {
                return errors;
            }

            var projectValue = result.View.GetInitialValue(nameof(Settings.Project)).Value;
            var repoValue = result.View.GetInitialValue(nameof(Settings.Repo)).Value;

            var developmentValue = result.View.GetInitialValue(nameof(Settings.DevelopmentStatuses))?.Value;
            var readyToReviewValue = result.View.GetInitialValue(nameof(Settings.ReadyToReviewStatuses))?.Value;
            var reviewValue = result.View.GetInitialValue(nameof(Settings.ReviewStatuses))?.Value;
            var readyValue = result.View.GetInitialValue(nameof(Settings.ReadyStatuses))?.Value;
            var closedValue = result.View.GetInitialValue(nameof(Settings.ClosedStatuses))?.Value;

            var staleTime = result.View.GetInitialValue(nameof(Settings.StaleTime)).Value;

            var jiraBlock = result.View.Blocks.First(x => x.BlockId == "settings-jiraOnOff");
            var jiraSupport = bool.Parse((jiraBlock as ActionBlock).Elements.First().Value);

            var gitlabBlock = result.View.Blocks.First(x => x.BlockId == "settings-gitlabOnOff");
            var isGitlab = bool.Parse((gitlabBlock as ActionBlock).Elements.First().Value);

            var gitlabUrl = result.View.GetInitialValue(nameof(Settings.RepoUrl))?.Value;
            var gitlabToken = result.View.GetInitialValue(nameof(Settings.GitlabToken))?.Value;

            var excludeBranches = result.View.GetInitialValue(nameof(Settings.ExcludeToBranches)).Value;
            var notifyUsers = result.View.GetInitialValue(nameof(Settings.NotifyUsers)).Value;

            var savedData = new Settings();
            if (!string.IsNullOrEmpty(id))
            {
                savedData = await _context.Settings.FirstAsync(x => x.Id == id);
            }

            savedData.ChannelId = channel;
            savedData.DevelopmentStatuses = developmentValue;
            savedData.JiraSupport = jiraSupport;
            savedData.IsGitlab = isGitlab;
            savedData.ReadyStatuses = readyValue;
            savedData.ReadyToReviewStatuses = readyToReviewValue;
            savedData.ReviewStatuses = reviewValue;
            savedData.ClosedStatuses = closedValue;
            savedData.Project = projectValue;
            savedData.Repo = repoValue;
            savedData.StaleTime = string.IsNullOrEmpty(staleTime) ? TimeSpan.Zero : TimeSpan.Parse(staleTime);
            savedData.ExcludeToBranches = excludeBranches;
            savedData.NotifyUsers = notifyUsers;
            savedData.GitlabToken = gitlabToken;
            savedData.RepoUrl = gitlabUrl;

            if (string.IsNullOrEmpty(id))
            {
                _context.Settings.Add(savedData);
            }

            await _context.SaveChangesAsync();
            await _slackApi.PostEphemeralMessageAsync(new SettingsSaved(channel, result.User.Id));

            return null;
        }

        public async Task ProcessViewBlockActionAsync(ViewBlockAction result)
        {
            if (result.Actions.Any(x => x.BlockId == "settings-jiraOnOff"))
            {
                var builder = new SettingsModalBuilder(result.View).ToggleJira();
                await _slackApi.EditModalAsync(new EditSettingsModal(builder));
            }

            if (result.Actions.Any(x => x.BlockId == "settings-gitlabOnOff"))
            {
                var builder = new SettingsModalBuilder(result.View).ToggleGitlab();
                await _slackApi.EditModalAsync(new EditSettingsModal(builder));
            }
        }

        private async Task OpenSettingsEditAsync(Settings settings, string triggerId)
        {
            var data = new SettingsModalBuilder().SetChannel(settings.ChannelId).SetTitle("Edit integration").AddDbId(settings.Id)
                .AddValue(nameof(Settings.JiraSupport), settings.JiraSupport.ToString())
                .AddValue(nameof(Settings.IsGitlab), settings.IsGitlab.ToString())
                .AddValue(nameof(Settings.DevelopmentStatuses), settings.DevelopmentStatuses)
                .AddValue(nameof(Settings.ReadyStatuses), settings.ReadyStatuses)
                .AddValue(nameof(Settings.ReadyToReviewStatuses), settings.ReadyToReviewStatuses)
                .AddValue(nameof(Settings.ReviewStatuses), settings.ReviewStatuses)
                .AddValue(nameof(Settings.ClosedStatuses), settings.ClosedStatuses)
                .AddValue(nameof(Settings.Project), settings.Project)
                .AddValue(nameof(Settings.Repo), settings.Repo)
                .AddValue(nameof(Settings.StaleTime), settings.StaleTime.ToString())
                .AddValue(nameof(Settings.ExcludeToBranches), settings.ExcludeToBranches)
                .AddValue(nameof(Settings.NotifyUsers), settings.NotifyUsers)
                .AddValue(nameof(Settings.RepoUrl), settings.RepoUrl)
                .AddValue(nameof(Settings.GitlabToken), settings.GitlabToken);

            await _slackApi.OpenModalAsync(new OpenModal
            {
                TriggerId = triggerId,
                View = data.Build()
            });
        }

        private async Task ProcessListSettingsActionsAsync(BlockAction result, string value)
        {
            var splits = value.Split('|');
            var action = splits[0];
            var id = splits[1];

            var settings = await _context.Settings.FirstOrDefaultAsync(x => x.Id == id);
            if (settings == null)
            {
                await _slackApi.PostResponseAsync(result.ResponseUrl, new DeleteResponse());
                return;
            }

            switch (action)
            {
                case "edit":
                    await OpenSettingsEditAsync(settings, result.TriggerId);
                    break;

                case "delete":
                    _context.Settings.Remove(settings);
                    await _context.SaveChangesAsync();
                    break;
            }

            await _slackApi.PostResponseAsync(result.ResponseUrl, new DeleteResponse());
        }

        private (string Key, string Value)? ValidateStatus(string status, string blockName, string name)
        {
            var errorText = $"{name} has invalid format";

            if (string.IsNullOrEmpty(status))
            {
                return null;
            }

            var splits = status.Split(new[] { "\n", "\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (!splits.Any())
            {
                return (blockName, errorText);
            }

            foreach (var split in splits)
            {
                var statusSplit = split.Split('|');
                if (statusSplit.Length != 3)
                {
                    return (blockName, errorText);
                }
            }

            return null;
        }

        private async Task<Message> ValidateSubmitFormAsync(ViewSubmission result, string channel, string id)
        {
            var errors = new SettingsErrors();

            var projectValue = result.View.GetInitialValue(nameof(Settings.Project)).Value;
            var repoValue = result.View.GetInitialValue(nameof(Settings.Repo)).Value;

            var staleTime = result.View.GetInitialValue(nameof(Settings.StaleTime))?.Value;

            if (!string.IsNullOrEmpty(staleTime) && !TimeSpan.TryParse(staleTime, out var _))
            {
                errors.AddError(nameof(Settings.StaleTime), "Stale time has invalid format");
            }

            if (await _context.Settings.AnyAsync(x => x.Id != id && x.ChannelId == channel && x.Project == projectValue && x.Repo == repoValue))
            {
                errors.AddError(nameof(Settings.Project), "Settings exist for current channel and repo");
                errors.AddError(nameof(Settings.Repo), "Settings exist for current channel and repo");
            }

            var developmentValue = result.View.GetInitialValue(nameof(Settings.DevelopmentStatuses))?.Value;
            var readyToReviewValue = result.View.GetInitialValue(nameof(Settings.ReadyToReviewStatuses))?.Value;
            var reviewValue = result.View.GetInitialValue(nameof(Settings.ReviewStatuses))?.Value;
            var readyValue = result.View.GetInitialValue(nameof(Settings.ReadyStatuses))?.Value;
            var closedValue = result.View.GetInitialValue(nameof(Settings.ClosedStatuses))?.Value;

            var statuses = new List<(string Key, string Value)?>();

            statuses.Add(ValidateStatus(developmentValue, nameof(Settings.DevelopmentStatuses), "Development statuses"));
            statuses.Add(ValidateStatus(readyToReviewValue, nameof(Settings.ReadyToReviewStatuses), "Ready to review statuses"));
            statuses.Add(ValidateStatus(reviewValue, nameof(Settings.ReviewStatuses), "Review statuses"));
            statuses.Add(ValidateStatus(readyValue, nameof(Settings.ReadyStatuses), "Ready statuses"));
            statuses.Add(ValidateStatus(closedValue, nameof(Settings.ClosedStatuses), "Closed statuses"));

            foreach (var status in statuses.Where(x => x.HasValue))
            {
                errors.AddError(status.Value.Key, status.Value.Value);
            }

            if (errors.HasErrors())
            {
                return errors.ToSlackMessage();
            }

            return null;
        }
    }
}