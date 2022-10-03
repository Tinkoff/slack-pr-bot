using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlackPrBot.DomainModels;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Api.Base;

namespace SlackPrBot.Services.Impl
{
    internal class JiraInteractiveService
    {
        private readonly JiraWorkflowService _jiraWorkflowService;

        private readonly Context _context;

        private readonly SlackApi _slackApi;

        private readonly SettingsService _settingsService;

        public JiraInteractiveService(JiraWorkflowService jiraWorkflowService, Context context, SlackApi slackApi, SettingsService settingsService)
        {
            _settingsService = settingsService;
            _slackApi = slackApi;
            _context = context;
            _jiraWorkflowService = jiraWorkflowService;
        }

        public Task ProcessBlockActionAsync(BlockAction result)
        {
            var action = result.Actions.FirstOrDefault(x => x.ActionId.StartsWith("jira"));
            if (action != null)
            {
                switch (action.ActionId)
                {
                    case "jira-move-issue":
                        return ProcessMoveActionAsync(result, action.Value);
                }
            }

            return Task.CompletedTask;
        }

        private async Task ProcessMoveActionAsync(BlockAction result, string value)
        {
            if (!int.TryParse(value, out var pullIdInt))
            {
                return;
            }

            var pullDb = await _context.PullRequests.FirstOrDefaultAsync(x => x.PullId == pullIdInt);
            if (pullDb == null)
            {
                await _slackApi.PostResponseAsync(result.ResponseUrl, new DeleteResponse());
                return;
            }

            var settings = await _settingsService.GetSettingsByAsync(pullDb.ProjectName, pullDb.RepoName);

            await _jiraWorkflowService.ApproveAsync(settings, result.User.Id, pullDb);
            await _slackApi.PostResponseAsync(result.ResponseUrl, new DeleteResponse());
        }
    }
}
