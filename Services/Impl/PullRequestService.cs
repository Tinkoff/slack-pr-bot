using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Api.Enums;
using SlackPrBot.Models.Stash;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackPrBot.Services.Impl
{
    internal class PullRequestService
    {
        private readonly Context _context;
        private readonly SettingsService _settingsService;
        private readonly SlackApi _slack;
        private readonly UserService _userService;
        private readonly JiraWorkflowService _jiraWorkflowService;
        private readonly StatisticService _statisticService;
        private readonly Credentials _atlassianCredentials;

        public PullRequestService(Context context, SlackApi slack, SettingsService settingsService, UserService userService, JiraWorkflowService jiraWorkflowService, StatisticService statisticService, IConfiguration config)
        {
            _slack = slack;
            _settingsService = settingsService;
            _userService = userService;
            _jiraWorkflowService = jiraWorkflowService;
            _statisticService = statisticService;
            _context = context;

            _atlassianCredentials = config.GetSection("AtlassianCredentials").Get<Credentials>();
        }

        public async Task PullRequestCreatedAsync(StashPullRequest pullRequest)
        {
            var actor = pullRequest.Actor;
            var pull = pullRequest.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;
            var settings = await _settingsService.GetSettingsByAsync(projectName, repoName);
            var now = DateTime.Now;

            var taskId = GetTaskId(pullRequest);

            var author = await _userService.GetUserAsync(actor.EmailAddress);

            var dbPull = new PullRequest
            {
                PullId = pull.Id,
                Iid = pull.Iid,
                FromRef = pull.FromRef.DisplayId,
                ProjectName = projectName,
                RepoName = repoName,
                Created = now,
                LastActiveDate = now,
                Author = author,
                TaskId = taskId
            };

            settings = settings.Where(x => !x.GetExcludeBranches().Contains(pull.ToRef.DisplayId)).ToArray();

            foreach (var settingsItem in settings)
            {
                var ts = await _slack.PostMessageAsync(new CreatePullRequest
                {
                    Channel = settingsItem.ChannelId,
                    Description = pull.Description,
                    ProjectName = projectName,
                    RepoName = repoName,
                    Name = pull.Title,
                    PullId = pull.Id,
                    From = pull.FromRef.DisplayId,
                    To = pull.ToRef.DisplayId,
                    AlternativeName = actor.AlternativeName,
                    TaskId = taskId,
                    CreatorUserId = author.SlackUserId,
                    Iid = pull.Iid,
                    IsGitlab = settingsItem.IsGitlab,
                    BaseRepoPath = settingsItem.RepoUrl,
                    JiraUrl = _atlassianCredentials.Url
                });

                dbPull.ChannelMap.Add(new PullRequestChannel
                {
                    SlackTs = ts,
                    Settings = settingsItem
                });

                if (!string.IsNullOrEmpty(settingsItem.NotifyUsers))
                {
                    var emailsOrGroups = settingsItem.NotifyUsers.Split(',').Select(x => x?.Trim()).Where(x => !string.IsNullOrEmpty(x));
                    var mentions = await _userService.ParseLoginsAndGroupsAsync(emailsOrGroups.ToArray());

                    await _slack.PostMessageAsync(new PullCreated
                    {
                        Channel = settingsItem.ChannelId,
                        AuthorName = actor.Name,
                        ProjectName = projectName,
                        RepoName = repoName,
                        ThreadTs = ts,
                        NotifyUsers = mentions,
                        PullId = dbPull.PullId,
                        Iid = dbPull.Iid,
                        IsGitlab = settingsItem.IsGitlab,
                        BaseRepoPath = settingsItem.RepoUrl
                    });
                }
            }

            if (settings.Any())
            {
                _context.PullRequests.Add(dbPull);

                await _context.SaveChangesAsync();

                await _jiraWorkflowService.ReadyToReviewAsync(settings, author.SlackUserId, dbPull);
                await _statisticService.PullRequestCreatedAsync(dbPull, author.Login);
            }
        }

        public async Task PullRequestReviewerStatusChangedAsync(StashPullRequest pullRequest, ReviewersStatus status)
        {
            var pull = pullRequest.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;

            var settings = await _settingsService.GetSettingsByAsync(projectName, repoName);
            var pullDb = await _context.PullRequests.Where(x => x.PullId == pull.Id && x.RepoName == repoName && x.ProjectName == projectName)
                                                    .FirstAsync();
            var reviewerUser = await _userService.GetUserAsync(pullRequest.Actor.EmailAddress);

            await _context.SaveChangesAsync();

            await _userService.AddToWatchListAsync(projectName, repoName, pull.Id, pullRequest.Actor.EmailAddress);
            await PullRequestReviewerStatusChangedInternalAsync(pullRequest.PullRequest, pullRequest.Actor, status);

            await _context.SaveChangesAsync();

            if (status == ReviewersStatus.Approved)
            {
                await _jiraWorkflowService.ReviewAsync(settings, pullDb.TaskId, reviewerUser);
                await _jiraWorkflowService.SendApproveMessageAsync(settings, reviewerUser.SlackUserId, pullDb);

                var author = await _context.PullRequests.Where(x => x.Id == pullDb.Id).Select(x => x.Author).FirstAsync();
                await _statisticService.PullRequestApprovedAsync(pullDb, reviewerUser.Login, author.Login);
            }

            if (status == ReviewersStatus.Unapproved)
            {
                await _jiraWorkflowService.ReviewAsync(settings, pullDb.TaskId, reviewerUser);
                await _jiraWorkflowService.DeclineAsync(settings, reviewerUser, pullDb);

                var author = await _context.PullRequests.Where(x => x.Id == pullDb.Id).Select(x => x.Author).FirstAsync();
                await _statisticService.PullRequestDeclinedAsync(pullDb, reviewerUser.Login, author.Login);
            }
        }

        public async Task PullRequestReviewersUpdateAsync(ReviewersChanged reviewersChanged)
        {
            var pull = reviewersChanged.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;
            var settings = await _settingsService.GetSettingsByAsync(projectName, repoName);

            try
            {
                foreach (var reviewer in reviewersChanged.RemovedReviewers)
                {
                    await _userService.RemoveFromWatchListAsync(projectName, repoName, pull.Id, reviewer.EmailAddress);
                    await PullRequestReviewerStatusChangedInternalAsync(reviewersChanged.PullRequest, reviewer, ReviewersStatus.RemoveApproval);
                }

                if (reviewersChanged.RemovedReviewers.Any())
                {
                    var pullDb = await _context.PullRequests.Where(x => x.PullId == pull.Id && x.RepoName == repoName && x.ProjectName == projectName)
                                                  .FirstAsync();
                    await _jiraWorkflowService.ReadyToReviewAsync(settings, null, pullDb);
                }
            }
            catch (Exception)
            {

            }

            try
            {
                foreach (var reviewer in reviewersChanged.AddedReviewers)
                {
                    await _userService.AddToWatchListAsync(projectName, repoName, pull.Id, reviewer.EmailAddress);
                }

                if (reviewersChanged.AddedReviewers.Any())
                {
                    var reviewer = reviewersChanged.AddedReviewers.First();
                    var taskAuthorInfo = await _context.PullRequests.Where(x => x.PullId == pull.Id && x.RepoName == repoName && x.ProjectName == projectName)
                                                        .Select(x => new { x.TaskId, AuthorEmail = x.Author.Email })
                                                        .FirstAsync();
                    if (reviewer.EmailAddress != taskAuthorInfo.AuthorEmail)
                    {
                        await _jiraWorkflowService.ReviewAsync(settings, taskAuthorInfo.TaskId, await _userService.GetUserAsync(reviewer.EmailAddress));
                    }
                }
            }
            catch (Exception)
            {

            }

            await _context.SaveChangesAsync();
        }

        public async Task PullRequestStatusChangedAsync(StashPullRequest pullRequest, PullStatus status)
        {
            var actor = pullRequest.Actor;
            var pull = pullRequest.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;

            var dbPull = await _context.PullRequests.FirstOrDefaultAsync(x => x.ProjectName == projectName && x.RepoName == repoName && x.PullId == pull.Id);
            if (dbPull == null)
            {
                return;
            }

            var slackMessageInfos = await _context.PullRequestsChannel.Where(x => x.PullRequest.Id == dbPull.Id).Select(x => new { x.SlackTs, Channel = x.Settings.ChannelId }).ToListAsync();

            foreach (var slackMessageInfo in slackMessageInfos)
            {
                var message = await _slack.FindMessageByTimestampAsync(slackMessageInfo.Channel, slackMessageInfo.SlackTs);

                var updatePullRequest = new UpdatePullRequest(message, slackMessageInfo.Channel, _atlassianCredentials.Url);

                updatePullRequest.SetStatus(status);

                await _slack.UpdateMessageAsync(updatePullRequest);
            }

            var author = await _context.PullRequests.Where(x => x.Id == dbPull.Id).Select(x => x.Author).FirstOrDefaultAsync();

            _context.PullRequests.Remove(dbPull);

            await _context.SaveChangesAsync();

            if (status == PullStatus.Merged)
            {
                var mergerUser = await _userService.GetUserAsync(pullRequest.Actor.EmailAddress);
                var settings = await _settingsService.GetSettingsByAsync(projectName, repoName);
                await _jiraWorkflowService.MergeAsync(settings, dbPull.TaskId);

                if (author != null)
                {
                    await _statisticService.PullRequestMergedAsync(dbPull, mergerUser.Login, author.Login);
                }


            }

            if ((status == PullStatus.Deleted || status == PullStatus.Declined) && author != null)
            {
                var mergerUser = await _userService.GetUserAsync(pullRequest.Actor.EmailAddress);
                await _statisticService.PullRequestClosedAsync(dbPull, mergerUser.Login, author.Login);
            }
        }

        public async Task PullRequestUpdatedAsync(StashPullRequest pullRequest)
        {
            var actor = pullRequest.Actor;
            var pull = pullRequest.PullRequest;
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;
            var now = DateTime.Now;

            var taskId = GetTaskId(pullRequest);

            var dbPull = await _context.PullRequests.FirstAsync(x => x.ProjectName == projectName && x.RepoName == repoName && x.PullId == pull.Id);
            var slackMessageInfos = await _context.PullRequestsChannel.Where(x => x.PullRequest.Id == dbPull.Id).Select(x => new { x.SlackTs, Channel = x.Settings.ChannelId }).ToListAsync();

            foreach (var slackMessageInfo in slackMessageInfos)
            {
                var message = await _slack.FindMessageByTimestampAsync(slackMessageInfo.Channel, slackMessageInfo.SlackTs);

                var updatePullRequest = new UpdatePullRequest(message, slackMessageInfo.Channel, _atlassianCredentials.Url);

                updatePullRequest
                    .ChangeDescription(pull.Description)
                    .ChangeName(pull.Title)
                    .UpdateFromTo(pull.FromRef.DisplayId, pull.ToRef.DisplayId)
                    .UpdateTaskId(taskId);

                await _slack.UpdateMessageAsync(updatePullRequest);
            }

            dbPull.LastActiveDate = now;
            dbPull.TaskId = taskId;

            await _context.SaveChangesAsync();
        }

        private string GetTaskId(StashPullRequest pullRequest)
        {
            var pull = pullRequest.PullRequest;
            var name = pull.Title;
            var fromRef = pull.FromRef.DisplayId;
            var regexp = new Regex("((?<!([A-Z]{1,10})-?)[A-Z]+-\\d+)");

            var nameMatch = regexp.Match(name);
            if (nameMatch.Length > 0)
            {
                return nameMatch.Value;
            }

            var fromRefMatch = regexp.Match(fromRef);
            return fromRefMatch.Length > 0 ? fromRefMatch.Value : "";
        }

        private async Task PullRequestReviewerStatusChangedInternalAsync(Models.Stash.Base.PullRequest pull, Models.Stash.Base.User reviewer, ReviewersStatus status)
        {
            var projectName = pull.ToRef.Repository.Project.Key;
            var repoName = pull.ToRef.Repository.Slug;
            var now = DateTime.Now;

            var user = await _userService.GetUserAsync(reviewer.EmailAddress);
            var author = (await _userService.GetUserAsync(pull.Author.User.EmailAddress))?.SlackUserId;
            var dbPull = await _context.PullRequests.FirstAsync(x => x.ProjectName == projectName && x.RepoName == repoName && x.PullId == pull.Id);
            var slackMessageInfos = await _context.PullRequestsChannel.Where(x => x.PullRequest.Id == dbPull.Id).Select(x => new
            {
                x.Settings.IsGitlab,
                x.SlackTs,
                Channel = x.Settings.ChannelId,
                x.Settings.RepoUrl
            }).ToListAsync();
            var reviewerDb = await _context.Reviewers.FirstOrDefaultAsync(x => x.PullRequest.Id == dbPull.Id && x.User.Id == user.Id);

            if ((status == ReviewersStatus.RemoveApproval && reviewerDb != null) ||
                 (status != ReviewersStatus.RemoveApproval && reviewerDb?.Status != (DomainModels.Enum.ReviewersStatus)status))
            {
                foreach (var slackMessageInfo in slackMessageInfos)
                {
                    var message = await _slack.FindMessageByTimestampAsync(slackMessageInfo.Channel, slackMessageInfo.SlackTs);

                    var updatePullRequest = new UpdatePullRequest(message, slackMessageInfo.Channel, _atlassianCredentials.Url);

                    updatePullRequest.UpdateUser(status, user?.SlackUserId, user?.Login);

                    await _slack.UpdateMessageAsync(updatePullRequest);
                    await _slack.PostMessageAsync(new ReviewerStatusChanged
                    {
                        Channel = slackMessageInfo.Channel,
                        ProjectName = projectName,
                        RepoName = repoName,
                        PullId = pull.Id,
                        ReviewerName = reviewer.Name,
                        AuthorId = author,
                        ThreadTs = slackMessageInfo.SlackTs,
                        Status = status,
                        Iid = pull.Iid,
                        IsGitlab = slackMessageInfo.IsGitlab,
                        BaseRepoPath = slackMessageInfo.RepoUrl
                    });
                }

                dbPull.LastActiveDate = now;

                if (status != ReviewersStatus.RemoveApproval)
                {
                    if (reviewerDb == null)
                    {
                        reviewerDb = new Reviewer
                        {
                            PullRequest = dbPull,
                            User = user
                        };
                        _context.Reviewers.Add(reviewerDb);
                    }

                    reviewerDb.Status = (DomainModels.Enum.ReviewersStatus)status;
                }
                else
                {
                    _context.Reviewers.Remove(reviewerDb);
                }
            }
        }
    }
}