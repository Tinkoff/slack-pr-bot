using System;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using SlackPrBot.Models.Slack;
using SlackPrBot.Models.Slack.Api.Base;
using SlackPrBot.Models.Slack.Request;

namespace SlackPrBot.Services.Impl
{
    internal class SlackInteractiveService : ISlackInteractiveService
    {
        private readonly SetupInteractiveService _setupInteractiveService;

        private readonly JiraInteractiveService _jiraInteractiveService;

        public SlackInteractiveService(SetupInteractiveService setupInteractiveService, JiraInteractiveService jiraInteractiveService)
        {
            _jiraInteractiveService = jiraInteractiveService;
            _setupInteractiveService = setupInteractiveService;
        }

        public async Task<Message> ProcessInteractiveAsync(InteractiveRequest request)
        {
            var jobject = JObject.Parse(request.Payload);
            switch (jobject.Value<string>("type"))
            {
                case "view_submission":
                    return await ProcessViewSubmissionAsync(jobject);

                case "block_actions":
                    await ProcessBlockActionAsync(jobject);
                    return null;
            }

            return null;
        }

        private Task ProcessBlockActionAsync(JObject json)
        {
            if (json.ContainsKey("view") && json["view"].HasValues)
            {
                var result = json.ToObject<ViewBlockAction>();

                switch ((ModalCallbackId)Enum.Parse(typeof(ModalCallbackId), result.View.CallbackId))
                {
                    case ModalCallbackId.CreateSettings:
                        return _setupInteractiveService.ProcessViewBlockActionAsync(result);
                }
            }

            var blockAction = json.ToObject<BlockAction>();
            var actionId = json["actions"][0]["action_id"].Value<string>();

            if (actionId.StartsWith("settings"))
            {
                return _setupInteractiveService.ProcessBlockActionAsync(blockAction);
            }
            if (actionId.StartsWith("jira"))
            {
                return _jiraInteractiveService.ProcessBlockActionAsync(blockAction);
            }

            return Task.CompletedTask;
        }

        private async Task<Message> ProcessViewSubmissionAsync(JObject json)
        {
            var result = json.ToObject<ViewSubmission>();

            switch ((ModalCallbackId)Enum.Parse(typeof(ModalCallbackId), result.View.CallbackId))
            {
                case ModalCallbackId.CreateSettings:
                    return await _setupInteractiveService.ProcessSubmitAsync(result);
            }

            return null;
        }
    }
}