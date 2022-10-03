using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SlackPrBot.DomainModels;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Api.ModalViewBuilders;
using SlackPrBot.Models.Slack.Request;

namespace SlackPrBot.Services.Impl
{
    internal class SetupSlashCommandService
    {
        private readonly Context _context;
        private readonly SettingsService _settingsService;
        private readonly SlackApi _slackApi;

        public SetupSlashCommandService(Context context, SlackApi slackApi, SettingsService settingsService)
        {
            _context = context;
            _slackApi = slackApi;
            _settingsService = settingsService;
        }

        public async Task ProcessSlashAsync(SlackSlashRequest request, string[] commands)
        {
            if ((commands.Any() && commands[0] == "add") || !await _context.Settings.AnyAsync(x => x.ChannelId == request.ChannelId))
            {
                var builder = new SettingsModalBuilder().SetChannel(request.ChannelId);
                await _slackApi.OpenModalAsync(new OpenModal
                {
                    TriggerId = request.TriggerId,
                    View = builder.Build()
                });
            }
            else
            {
                await _slackApi.PostEphemeralMessageAsync(await _settingsService.GenerateSettingsListAsync(request.ChannelId, request.UserId));
            }
        }
    }
}