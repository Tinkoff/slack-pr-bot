using System.Linq;
using System;
using System.Threading.Tasks;
using Atlassian.Jira;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models.Jira.Enums;
using SlackPrBot.Models.Slack.Api.Jira;
using Microsoft.Extensions.Logging;
using SlackPrBot.Models;
using Microsoft.Extensions.Configuration;

namespace SlackPrBot.Services.Impl
{
    internal class JiraWorkflowService
    {
        private readonly JiraInternalService _jiraInternalService;
        private readonly SlackApi _slackApi;
        private readonly ILogger<JiraWorkflowService> _log;

        private readonly Credentials _atlassianCredentials;

        public JiraWorkflowService(JiraInternalService jiraInternalService, SlackApi slackApi, ILogger<JiraWorkflowService> log, IConfiguration config)
        {
            _jiraInternalService = jiraInternalService;
            _slackApi = slackApi;
            _log = log;

            _atlassianCredentials = config.GetSection("AtlassianCredentials").Get<Credentials>();
        }

        public async Task ReadyToReviewAsync(Settings[] settings, string slackUserId, PullRequest pull)
        {
            var settingsWithJiraSupport = settings.FirstOrDefault(x => x.JiraSupport);

            if (settingsWithJiraSupport == null || string.IsNullOrEmpty(pull.TaskId) || string.IsNullOrEmpty(slackUserId)) return;

            var client = _jiraInternalService.Client;
            if (client == null)
            {
                return;
            }

            var issue = await client.Issues.GetIssueAsync(pull.TaskId);
            if (issue == null)
            {
                return;
            }

            try
            {

                await ChangeJiraStatusAsync(issue, settingsWithJiraSupport, JiraStatus.ReadyToReview);

                if (!string.IsNullOrEmpty(slackUserId))
                {
                    await _slackApi.PostMessageAsync(new LogWork
                    {
                        TaskId = pull.TaskId,
                        Channel = slackUserId,
                        JiraId = issue.JiraIdentifier,
                        PullId = pull.PullId,
                        Project = pull.ProjectName,
                        Repo = pull.RepoName,
                        IsGitlab = settingsWithJiraSupport.IsGitlab,
                        BaseRepoPath = settingsWithJiraSupport.RepoUrl,
                        JiraUrl = _atlassianCredentials.Url,
                        Iid = pull.Iid
                    });
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in ReadyToReview status change");
            }
        }

        public async Task ReviewAsync(Settings[] settings, string taskId, User reviewer)
        {
            var settingsWithJiraSupport = settings.FirstOrDefault(x => x.JiraSupport);

            if (settingsWithJiraSupport == null || string.IsNullOrEmpty(taskId)) return;

            var client = _jiraInternalService.Client;
            if (client == null)
            {
                return;
            }

            var issue = await client.Issues.GetIssueAsync(taskId);
            if (issue == null)
            {
                return;
            }

            try
            {
                var changed = await ChangeJiraStatusAsync(issue, settingsWithJiraSupport, JiraStatus.Review);

                if (changed)
                {
                    issue.Assignee = reviewer.Login;

                    await issue.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in Review status change");
            }
        }

        public async Task MergeAsync(Settings[] settings, string taskId)
        {
            var settingsWithJiraSupport = settings.FirstOrDefault(x => x.JiraSupport && x.IsGitlab);

            if (settingsWithJiraSupport == null || string.IsNullOrEmpty(taskId)) return;

            var client = _jiraInternalService.Client;
            if (client == null)
            {
                return;
            }

            var issue = await client.Issues.GetIssueAsync(taskId);
            if (issue == null)
            {
                return;
            }

            try
            {

                await ChangeJiraStatusAsync(issue, settingsWithJiraSupport, JiraStatus.Closed);

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in Merge status change");
            }
        }

        public async Task SendApproveMessageAsync(Settings[] settings, string slackUserId, PullRequest pull)
        {
            var settingsWithJiraSupport = settings.FirstOrDefault(x => x.JiraSupport);

            if (settingsWithJiraSupport == null || string.IsNullOrEmpty(pull.TaskId) || string.IsNullOrEmpty(slackUserId)) return;

            var client = _jiraInternalService.Client;
            if (client == null)
            {
                return;
            }

            var issue = await client.Issues.GetIssueAsync(pull.TaskId);
            if (issue == null)
            {
                return;
            }

            var transitionName = GetWorkflowTransition(settingsWithJiraSupport, issue.Type.Name, issue.Status.Name, JiraStatus.Ready);
            if (string.IsNullOrEmpty(transitionName))
            {
                return;
            }

            await _slackApi.PostMessageAsync(new ReadyStatusMove
            {
                Channel = slackUserId,
                TaskId = pull.TaskId,
                PullId = pull.PullId,
                ProjectName = pull.ProjectName,
                RepoName = pull.RepoName,
                Iid = pull.Iid,
                IsGitlab = settingsWithJiraSupport.IsGitlab,
                BaseRepoPath = settingsWithJiraSupport.RepoUrl,
                JiraUrl = _atlassianCredentials.Url
            });
        }

        public async Task ApproveAsync(Settings[] settings, string slackUserId, PullRequest pull)
        {
            var settingsWithJiraSupport = settings.FirstOrDefault(x => x.JiraSupport);

            if (settingsWithJiraSupport == null || string.IsNullOrEmpty(pull.TaskId)) return;

            var client = _jiraInternalService.Client;
            if (client == null)
            {
                return;
            }

            var issue = await client.Issues.GetIssueAsync(pull.TaskId);
            if (issue == null)
            {
                return;
            }

            try
            {
                await ChangeJiraStatusAsync(issue, settingsWithJiraSupport, JiraStatus.Ready);
                await _slackApi.PostMessageAsync(new LogWork
                {
                    TaskId = pull.TaskId,
                    Channel = slackUserId,
                    JiraId = issue.JiraIdentifier,
                    PullId = pull.PullId,
                    Project = pull.ProjectName,
                    Repo = pull.RepoName,
                    IsGitlab = settingsWithJiraSupport.IsGitlab,
                    BaseRepoPath = settingsWithJiraSupport.RepoUrl,
                    JiraUrl = _atlassianCredentials.Url,
                    Iid = pull.Iid
                });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in Approve status change");
            }
        }

        public async Task DeclineAsync(Settings[] settings, User reviewer, PullRequest pull)
        {
            var settingsWithJiraSupport = settings.FirstOrDefault(x => x.JiraSupport);

            if (settingsWithJiraSupport == null || string.IsNullOrEmpty(pull.TaskId) || string.IsNullOrEmpty(reviewer.SlackUserId)) return;

            var client = _jiraInternalService.Client;
            if (client == null)
            {
                return;
            }

            var issue = await client.Issues.GetIssueAsync(pull.TaskId);
            if (issue == null)
            {
                return;
            }

            try
            {
                await ChangeJiraStatusAsync(issue, settingsWithJiraSupport, JiraStatus.Development);
                await _slackApi.PostMessageAsync(new LogWork
                {
                    TaskId = pull.TaskId,
                    Channel = reviewer.SlackUserId,
                    JiraId = issue.JiraIdentifier,
                    PullId = pull.PullId,
                    Project = pull.ProjectName,
                    Repo = pull.RepoName,
                    IsGitlab = settingsWithJiraSupport.IsGitlab,
                    BaseRepoPath = settingsWithJiraSupport.RepoUrl,
                    JiraUrl = _atlassianCredentials.Url,
                    Iid = pull.Iid
                });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error in Decline status change");
            }
        }

        private async Task<bool> ChangeJiraStatusAsync(Issue issue, Settings settings, JiraStatus targetStatus)
        {
            var transitionName = GetWorkflowTransition(settings, issue.Type.Name, issue.Status.Name, targetStatus);
            if (string.IsNullOrEmpty(transitionName) || transitionName == issue.Status.Name.ToLower())
            {
                return false;
            }

            await _jiraInternalService.Client.Issues.ExecuteWorkflowActionAsync(issue, transitionName, new WorkflowTransitionUpdates());

            await issue.RefreshAsync();
            return true;
        }

        private string GetWorkflowTransition(Settings settings, string jiraType, string statusNameFrom, JiraStatus targetStatus)
        {
            if (targetStatus == JiraStatus.Development)
            {
                return GetWorkflowTransition(settings.DevelopmentStatuses, jiraType, statusNameFrom);
            }
            if (targetStatus == JiraStatus.Ready)
            {
                return GetWorkflowTransition(settings.ReadyStatuses, jiraType, statusNameFrom);
            }
            if (targetStatus == JiraStatus.ReadyToReview)
            {
                return GetWorkflowTransition(settings.ReadyToReviewStatuses, jiraType, statusNameFrom);
            }
            if (targetStatus == JiraStatus.Review)
            {
                return GetWorkflowTransition(settings.ReviewStatuses, jiraType, statusNameFrom);
            }
            if (targetStatus == JiraStatus.Closed)
            {
                return GetWorkflowTransition(settings.ClosedStatuses, jiraType, statusNameFrom);
            }

            return null;
        }

        private string GetWorkflowTransition(string statusData, string jiraType, string statusName)
        {
            foreach (var entry in statusData.ToLower().Split(new[] { "\n", "\r", "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                var splits = entry.Split('|');
                if (splits[0] != jiraType.ToLower() || splits[1] != statusName.ToLower())
                {
                    continue;
                }

                return splits[2].ToLower();
            }

            return null;
        }
    }
}