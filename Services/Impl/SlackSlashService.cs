using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SlackPrBot.Models.Slack.Api;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Request;

namespace SlackPrBot.Services.Impl
{
    internal class SlackSlashService : ISlackSlashService
    {
        private readonly ILogger<SlackSlashService> _log;
        private readonly SetupSlashCommandService _setupSlashCommandService;
        private readonly SlackApi _slackApi;
        private readonly StatisticsSlashCommandService _statisticsSlashCommandService;

        public SlackSlashService(ILogger<SlackSlashService> log,
            SlackApi slackApi,
            SetupSlashCommandService setupSlashCommandService,
            StatisticsSlashCommandService statisticsSlashCommandService)
        {
            _log = log;
            _slackApi = slackApi;
            _setupSlashCommandService = setupSlashCommandService;
            _statisticsSlashCommandService = statisticsSlashCommandService;
        }

        public async Task ProcessSlashAsync(SlackSlashRequest request)
        {
            try
            {
                var commands = await GetCommandsAsync(request);
                var channels = await _slackApi.GetBotChannelsAsync();

                if (!await BotInChannelAsync(request, channels))
                    return;


                switch (commands[0])
                {
                    case "setup":
                        if (!await BotImAsync(request, channels))
                        {
                            await _setupSlashCommandService.ProcessSlashAsync(request, commands.Skip(1).ToArray());
                        }
                        break;
                    case "stat":
                        await _statisticsSlashCommandService.ProcessSlashAsync(request, commands.Skip(1).ToArray());
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while processing slash command");
            }
        }

        private async Task<bool> BotInChannelAsync(SlackSlashRequest request, Channel[] channels)
        {
            if (!channels.Any(x => x.Id == request.ChannelId))
            {
                await _slackApi.PostResponseAsync(request.ResponseUrl, new BotNotInChannel());
                return false;
            }

            return true;
        }

        private async Task<bool> BotImAsync(SlackSlashRequest request, Channel[] channels)
        {
            if (channels.Any(x => x.Id == request.ChannelId && !x.IsChannel && !x.IsGroup && !x.IsPrivate))
            {
                await _slackApi.PostResponseAsync(request.ResponseUrl, new BotInImChannel());
                return true;
            }

            return false;
        }

        private async Task<string[]> GetCommandsAsync(SlackSlashRequest request)
        {
            var text = request.Text;
            if (string.IsNullOrEmpty(text))
            {
                await _slackApi.PostResponseAsync(request.ResponseUrl, new WrongSlashCommand());
                throw new Exception("Wrong slash command");
            }

            var commands = text.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            if (commands[0] != "setup" && commands[0] != "stat")
            {
                await _slackApi.PostResponseAsync(request.ResponseUrl, new WrongSlashCommand());
                throw new Exception("Wrong slash command");
            }

            return commands;
        }
    }
}
