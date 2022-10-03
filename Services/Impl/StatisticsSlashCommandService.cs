using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Enum;
using SlackPrBot.Models;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Api.Stat;
using SlackPrBot.Models.Slack.Request;

namespace SlackPrBot.Services.Impl
{
    internal class StatisticsSlashCommandService
    {
        private readonly SlackApi _slackApi;

        private readonly Context _context;

        private readonly Credentials _atlassianCredentials;

        public StatisticsSlashCommandService(SlackApi slackApi, Context context, IConfiguration config)
        {
            _context = context;
            _slackApi = slackApi;
            _atlassianCredentials = config.GetSection("AtlassianCredentials").Get<Credentials>();
        }

        public Task ProcessSlashAsync(SlackSlashRequest request, string[] commands)
        {
            if (!commands.Any())
            {
                return _slackApi.PostResponseAsync(request.ResponseUrl, new WrongSlashCommand());
            }

            var isMy = commands.Length > 1 && commands[1] == "my";

            switch (commands[0])
            {
                case "open":
                    return ProcessOpenStatAsync(request, isMy);
                case "rtm":
                    return ProcessRtmStatAsync(request, isMy);

            }
            return Task.CompletedTask;
        }

        private async Task ProcessOpenStatAsync(SlackSlashRequest request, bool isMy)
        {
            var channels = await _slackApi.GetBotChannelsAsync();
            if (channels.Any(x => x.Id == request.ChannelId && !x.IsChannel && !x.IsGroup && !x.IsPrivate))
            {
                var allMyPrs = await _context.PullRequests
                .Where(x => x.Author.SlackUserId == request.UserId)
                .Where(x => !x.Reviewers.Any(r => r.Status == ReviewersStatus.Approved))
                .Select(x => new PrInfo
                {
                    PullId = x.PullId,
                    TaskId = x.TaskId,
                    RepoName = x.RepoName,
                    ProjectName = x.ProjectName,
                    TimeSinceLastAction = DateTime.Now - x.LastActiveDate,
                    Iid = x.Iid,
                    IsGitlab = x.ChannelMap.Any(c => c.Settings.IsGitlab),
                    BaseRepoPath = x.ChannelMap.Select(c => c.Settings.RepoUrl).FirstOrDefault()
                }).ToListAsync();

                if (!allMyPrs.Any())
                {
                    await _slackApi.PostResponseAsync(request.ResponseUrl, new NothingFound());
                    return;
                }

                await _slackApi.PostResponseAsync(request.ResponseUrl, new OpenPrStat { Tasks = allMyPrs, JiraUrl = _atlassianCredentials.Url });
                return;
            }

            var channelPrs = await _context.PullRequestsChannel
            .Where(x => x.Settings.ChannelId == request.ChannelId)
            .Select(x => new { x.PullRequest, x.Settings })
            .Where(x => !isMy || x.PullRequest.Author.SlackUserId == request.UserId)
            .Where(x => !x.PullRequest.Reviewers.Any(r => r.Status == ReviewersStatus.Approved))
            .Select(x => new PrInfo
            {
                PullId = x.PullRequest.PullId,
                TaskId = x.PullRequest.TaskId,
                RepoName = x.PullRequest.RepoName,
                ProjectName = x.PullRequest.ProjectName,
                TimeSinceLastAction = DateTime.Now - x.PullRequest.LastActiveDate,
                Iid = x.PullRequest.Iid,
                IsGitlab = x.Settings.IsGitlab,
                BaseRepoPath = x.Settings.RepoUrl
            }).ToListAsync();

            if (!channelPrs.Any())
            {
                await _slackApi.PostResponseAsync(request.ResponseUrl, new NothingFound());
                return;
            }

            await _slackApi.PostResponseAsync(request.ResponseUrl, new OpenPrStat { Tasks = channelPrs, JiraUrl = _atlassianCredentials.Url });
        }

        private async Task ProcessRtmStatAsync(SlackSlashRequest request, bool isMy)
        {
            var channels = await _slackApi.GetBotChannelsAsync();
            if (channels.Any(x => x.Id == request.ChannelId && !x.IsChannel && !x.IsGroup && !x.IsPrivate))
            {
                var allMyPrs = await _context.PullRequests
                .Where(x => x.Author.SlackUserId == request.UserId)
                .Where(x => x.Reviewers.Any(r => r.Status == ReviewersStatus.Approved))
                .Select(x => new PrInfo
                {
                    PullId = x.PullId,
                    TaskId = x.TaskId,
                    RepoName = x.RepoName,
                    ProjectName = x.ProjectName,
                    TimeSinceLastAction = DateTime.Now - x.LastActiveDate,
                    Iid = x.Iid,
                    IsGitlab = x.ChannelMap.Any(c => c.Settings.IsGitlab),
                    BaseRepoPath = x.ChannelMap.Select(c => c.Settings.RepoUrl).FirstOrDefault()
                }).ToListAsync();

                if (!allMyPrs.Any())
                {
                    await _slackApi.PostResponseAsync(request.ResponseUrl, new NothingFound());
                    return;
                }

                await _slackApi.PostResponseAsync(request.ResponseUrl, new ReadyToMergeStat { Tasks = allMyPrs, JiraUrl = _atlassianCredentials.Url });
                return;
            }

            var channelPrs = await _context.PullRequestsChannel
            .Where(x => x.Settings.ChannelId == request.ChannelId)
            .Select(x => new { x.PullRequest, x.Settings })
            .Where(x => !isMy || x.PullRequest.Author.SlackUserId == request.UserId)
            .Where(x => x.PullRequest.Reviewers.Any(r => r.Status == ReviewersStatus.Approved))
            .Select(x => new PrInfo
            {
                PullId = x.PullRequest.PullId,
                TaskId = x.PullRequest.TaskId,
                RepoName = x.PullRequest.RepoName,
                ProjectName = x.PullRequest.ProjectName,
                TimeSinceLastAction = DateTime.Now - x.PullRequest.LastActiveDate,
                Iid = x.PullRequest.Iid,
                IsGitlab = x.Settings.IsGitlab,
                BaseRepoPath = x.Settings.RepoUrl
            }).ToListAsync();

            if (!channelPrs.Any())
            {
                await _slackApi.PostResponseAsync(request.ResponseUrl, new NothingFound());
                return;
            }

            await _slackApi.PostResponseAsync(request.ResponseUrl, new ReadyToMergeStat { Tasks = channelPrs, JiraUrl = _atlassianCredentials.Url });
        }
    }
}
