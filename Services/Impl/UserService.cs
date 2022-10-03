using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models;

namespace SlackPrBot.Services.Impl
{
    internal class UserService
    {
        private readonly ILogger<UserService> _log;
        private readonly Context _context;
        private readonly SlackApi _slackApi;
        private readonly Cache _cache;

        public UserService(ILogger<UserService> log, Context context, SlackApi slackApi, Cache cache)
        {
            _log = log;
            _context = context;
            _slackApi = slackApi;
            _cache = cache;
        }

        public async Task AddToWatchListAsync(string project, string repo, int pullId, string email)
        {
            var user = await GetUserAsync(email);

            var pull = await _context.PullRequests.FirstOrDefaultAsync(x => x.ProjectName == project && x.RepoName == repo && x.PullId == pullId);
            if (pull == null)
            {
                return;
            }

            if (!await _context.PullRequestWatch.AnyAsync(x => x.User.Email == user.Email && x.PullRequest.Id == pull.Id))
            {
                _context.PullRequestWatch.Add(new PullRequestWatch
                {
                    Id = Guid.NewGuid().ToString(),
                    PullRequest = pull,
                    User = user
                });
            }
        }

        public async Task<User> GetUserAsync(string email, bool withoutCreate = false)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                return user;
            }

            var newUser = new User
            {
                Email = email
            };

            try
            {
                var userIdFromSlack = await _slackApi.FindUserIdByEmailAsync(email);
                if (userIdFromSlack != null)
                {
                    newUser.SlackUserId = userIdFromSlack;
                }
            }
            catch (Exception)
            {
                // gulp
            }

            if (!withoutCreate)
            {
                _context.Users.Add(newUser);
            }


            return newUser;
        }

        public async Task RemoveFromWatchListAsync(string project, string repo, int pullId, string email)
        {
            var user = await GetUserAsync(email);

            var pull = await _context.PullRequests.FirstOrDefaultAsync(x => x.ProjectName == project && x.RepoName == repo && x.PullId == pullId);

            if (pull == null)
            {
                return;
            }

            var watch = await _context.PullRequestWatch.FirstOrDefaultAsync(x => x.User.Email == user.Email && x.PullRequest.Id == pull.Id);
            if (watch != null)
            {
                _context.PullRequestWatch.Remove(watch);
            }
        }

        public async Task<string[]> ParseLoginsAndGroupsAsync(string[] emailsAndGroups)
        {
            var result = new List<string>();

            try
            {
                var groups = _cache.SlackUserGroups;
                if (_cache.LastUserGroupCheck == DateTime.MinValue || _cache.SlackUserGroups == null || (DateTime.Now - _cache.LastUserGroupCheck).TotalMinutes > 5)
                {

                    _cache.SlackUserGroups = await _slackApi.GetUserGroupsAsync();
                    _cache.LastUserGroupCheck = DateTime.Now;
                    groups = _cache.SlackUserGroups;
                }

                foreach (var emailOrGroup in emailsAndGroups.Distinct())
                {
                    var group = groups.FirstOrDefault(x => x.Name == emailOrGroup || x.Handle == emailOrGroup);
                    if (group != null)
                    {
                        result.Add($"<!subteam^{group.Id}>");
                        continue;
                    }

                    try
                    {
                        var user = await GetUserAsync(emailOrGroup, true);
                        if (user != null && !string.IsNullOrEmpty(user.SlackUserId))
                        {
                            result.Add($"<@{user.SlackUserId}>");
                        }

                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, "user search error");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "group search error");
            }

            return result.ToArray();
        }
    }
}