using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using SlackPrBot.DomainModels;
using SlackPrBot.Models;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Model;

namespace SlackPrBot.Services.Impl
{
    internal class NotificationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<NotificationService> _log;
        private readonly Context _context;
        private readonly SlackApi _slack;
        private readonly GitlabApi _gitlab;
        private readonly Credentials _atlassianCredentials;

        public NotificationService(ILogger<NotificationService> log, Context context, SlackApi slack, GitlabApi gitlab, IConfiguration config)
        {
            _gitlab = gitlab;
            _log = log;
            _context = context;
            _slack = slack;
            _config = config;

            _atlassianCredentials = config.GetSection("AtlassianCredentials").Get<Credentials>();
        }

        public async Task CheckOverdueAsync()
        {
            var checkTime = _config.GetValue<TimeSpan>("StaleTimerCheck");
            var now = DateTime.Now;

            var prs = await _context.PullRequestsChannel
                .Select(x => new
                {
                    PullRequestChannelId = x.Id,
                    x.LastNotificationDate,
                    x.Settings.ChannelId,
                    x.Settings.StaleTime,
                    x.PullRequest.LastActiveDate,
                    x.PullRequest.TaskId,
                    x.PullRequest.PullId,
                    x.PullRequest.RepoName,
                    x.PullRequest.ProjectName,
                    x.PullRequest.Iid,
                    DbPrId = x.PullRequest.Id,
                    x.Settings.IsGitlab,
                    x.Settings.GitlabToken,
                    x.Settings.RepoUrl
                }).ToListAsync();

            var gitlabPrs = prs.Where(x => x.IsGitlab && !string.IsNullOrEmpty(x.GitlabToken)).ToList();
            foreach (var gitlabPR in gitlabPrs)
            {
                try
                {
                    var gitlabStatus = await _gitlab.GetMergeRequestAsync(gitlabPR.RepoUrl, gitlabPR.GitlabToken, $"{gitlabPR.ProjectName}/{gitlabPR.RepoName}", (int)gitlabPR.Iid);
                    if (gitlabStatus.State != "opened")
                    {
                        var dbPr = await _context.PullRequests.FirstOrDefaultAsync(x => x.Id == gitlabPR.DbPrId);
                        if (dbPr != null)
                        {
                            _context.PullRequests.Remove(dbPr);
                            await _context.SaveChangesAsync();
                        }

                        prs.Remove(gitlabPR);
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Error while clearing gitlab MRs");
                }

            }

            var groupPrs = prs.GroupBy(x => new { x.ChannelId, x.StaleTime });

            foreach (var pr in groupPrs)
            {

                var staleTime = pr.Key.StaleTime;
                if (staleTime != TimeSpan.Zero && checkTime != TimeSpan.Zero)
                {
                    var filterPrs = pr.Where(x => DateTime.Now - x.LastActiveDate >= staleTime
                          && (!x.LastNotificationDate.HasValue || DateTime.Now - x.LastNotificationDate.Value >= checkTime))
                              .Select(x => new StaleNotificationTask
                              {
                                  Overdue = DateTime.Now - x.LastActiveDate,
                                  TaskId = x.TaskId,
                                  PullId = x.PullId,
                                  Id = x.PullRequestChannelId,
                                  ProjectName = x.ProjectName,
                                  RepoName = x.RepoName,
                                  Iid = x.Iid,
                                  IsGitlab = x.IsGitlab,
                                  BaseRepoPath = x.RepoUrl
                              }).ToList();

                    if (filterPrs.Any())
                    {
                        try
                        {
                            await _slack.PostMessageAsync(new StaleNotification
                            {
                                Channel = pr.Key.ChannelId,
                                Tasks = filterPrs,
                                JiraUrl = _atlassianCredentials.Url
                            });
                        }
                        catch (Exception ex)
                        {
                            _log.LogError(ex, $"Error while posting to channel {pr.Key.ChannelId}"); ;
                        }
                    }

                    foreach (var filterPr in filterPrs)
                    {
                        var dbPullChannel = await _context.PullRequestsChannel.FirstAsync(x => x.Id == filterPr.Id);
                        dbPullChannel.LastNotificationDate = now;
                    }

                    if (filterPrs.Any())
                    {
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
    }
}