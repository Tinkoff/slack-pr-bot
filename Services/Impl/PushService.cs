using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SlackPrBot.DomainModels;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Stash;

namespace SlackPrBot.Services.Impl
{
    internal class PushService
    {
        private readonly Context _context;
        private readonly SlackApi _slack;
        private readonly JiraWorkflowService _jiraWorkflowService;
        private readonly SettingsService _settingsService;
        private readonly UserService _userService;

        public PushService(Context context, SlackApi slack, JiraWorkflowService jiraWorkflowService, SettingsService settingsService, UserService userService)
        {
            _context = context;
            _slack = slack;
            _jiraWorkflowService = jiraWorkflowService;
            _settingsService = settingsService;
            _userService = userService;
        }

        public async Task ChangesPushedAsync(Push push)
        {
            var actor = push.Actor;
            var projectName = push.Repository.Project.Key;
            var repoName = push.Repository.Slug;
            var now = DateTime.Now;
            var settings = await _settingsService.GetSettingsByAsync(projectName, repoName);

            var changes = push.Changes.GroupBy(x => x.Ref.DisplayId).Select(x => new { Ref = x.Key, Hash = x.Select(r => r.ToHash).First() });
            var refs = changes.Select(x => x.Ref);

            var dbPulls = await _context.PullRequests.Where(x => refs.Contains(x.FromRef) && x.RepoName == repoName && x.ProjectName == projectName).ToListAsync();
            var pullsIds = dbPulls.Select(x => x.Id);
            var watches = await _context.PullRequestWatch.Where(x => pullsIds.Contains(x.PullRequest.Id)).Select(x => new { x.PullRequest.Id, x.User }).ToListAsync();

            foreach (var dbPull in dbPulls)
            {
                var change = changes.First(x => x.Ref == dbPull.FromRef);
                if (change.Hash == "0000000000000000000000000000000000000000")
                {
                    continue;
                }

                var slackInfos = await _context.PullRequestsChannel.Where(x => x.PullRequest.Id == dbPull.Id).Select(x => new { x.SlackTs, x.Settings.ChannelId, x.Settings.IsGitlab, x.Settings.RepoUrl }).ToListAsync();
                var slackUserWatches = watches.Where(x => x.Id == dbPull.Id).Select(x => x.User).ToList();

                foreach (var slackInfo in slackInfos)
                {
                    var message = await _slack.FindMessageByTimestampAsync(slackInfo.ChannelId, slackInfo.SlackTs);

                    if (message == null || string.IsNullOrEmpty(message.Ts))
                    {
                        continue;
                    }

                    await _slack.PostMessageAsync(new CommitAdded
                    {
                        Channel = slackInfo.ChannelId,
                        CommitAuthorName = actor.Name,
                        CommitId = change.Hash,
                        ProjectName = dbPull.ProjectName,
                        RepoName = dbPull.RepoName,
                        ThreadTs = slackInfo.SlackTs,
                        WatchUsers = slackUserWatches,
                        PullId = dbPull.PullId,
                        Iid = dbPull.Iid,
                        IsGitlab = slackInfo.IsGitlab,
                        BaseRepoPath = slackInfo.RepoUrl
                    });
                }

                dbPull.LastActiveDate = now;

                await _context.SaveChangesAsync();

                var slackAuthorId = await _context.PullRequests.Where(x => x.PullId == dbPull.PullId && x.Author.Email == actor.EmailAddress).Select(x => x.Author.SlackUserId).FirstOrDefaultAsync();

                if (!string.IsNullOrEmpty(slackAuthorId))
                {
                    await _jiraWorkflowService.ReadyToReviewAsync(settings, slackAuthorId, dbPull);
                }
            }
        }
    }
}
