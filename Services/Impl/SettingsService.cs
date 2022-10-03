using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SlackPrBot.DomainModels;
using SlackPrBot.DomainModels.Entities;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Model;

namespace SlackPrBot.Services.Impl
{
    internal class SettingsService
    {
        private readonly Context _context;

        public SettingsService(Context context)
        {
            _context = context;
        }

        public async Task<SettingsList> GenerateSettingsListAsync(string channel, string userId, bool replace = false)
        {
            var settings = await _context.Settings.Where(x => x.ChannelId == channel).Select(x => new SettingsEditInfo
            {
                Id = x.Id,
                Project = x.Project,
                Repo = x.Repo,
                IsGitlab = x.IsGitlab,
                BaseRepoPath = x.RepoUrl
            }).ToListAsync();


            return new SettingsList
            {
                ReplaceMessage = replace,
                Channel = channel,
                Settings = settings,
                UserId = userId
            };
        }

        public async Task<Settings[]> GetSettingsByAsync(string projectName, string repoName)
        {
            return await _context.Settings.Where(x => x.Project == projectName && x.Repo == repoName).ToArrayAsync();
        }
    }
}