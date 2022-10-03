using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using SlackPrBot.Models.Slack.Api.Enums;
using SlackPrBot.Models.Stash;

namespace SlackPrBot.Services.Impl
{
    internal class StashEventService : IStashEventService
    {
        private readonly CommentService _commentService;
        private readonly ILogger<StashEventService> _logger;
        private readonly PullRequestService _pullRequestService;
        private readonly PushService _pushService;

        public StashEventService(ILogger<StashEventService> logger, PullRequestService pullRequestService, PushService pushService, CommentService commentService)
        {
            _pullRequestService = pullRequestService;
            _pushService = pushService;
            _commentService = commentService;
            _logger = logger;
        }

        public async Task StashEventAsync(JObject eventData)
        {
            try
            {
                switch (eventData.Value<string>("eventKey"))
                {
                    case "pr:opened":
                        await _pullRequestService.PullRequestCreatedAsync(eventData.ToObject<StashPullRequest>());
                        break;

                    case "pr:modified":
                        await _pullRequestService.PullRequestUpdatedAsync(eventData.ToObject<StashPullRequest>());
                        break;

                    case "pr:declined":
                        await _pullRequestService.PullRequestStatusChangedAsync(eventData.ToObject<StashPullRequest>(), PullStatus.Declined);
                        break;

                    case "pr:deleted":
                        await _pullRequestService.PullRequestStatusChangedAsync(eventData.ToObject<StashPullRequest>(), PullStatus.Deleted);
                        break;

                    case "pr:merged":
                        await _pullRequestService.PullRequestStatusChangedAsync(eventData.ToObject<StashPullRequest>(), PullStatus.Merged);
                        break;

                    case "pr:reviewer:approved":
                        await _pullRequestService.PullRequestReviewerStatusChangedAsync(eventData.ToObject<StashPullRequest>(), ReviewersStatus.Approved);
                        break;

                    case "pr:reviewer:unapproved":
                        await _pullRequestService.PullRequestReviewerStatusChangedAsync(eventData.ToObject<StashPullRequest>(), ReviewersStatus.RemoveApproval);
                        break;

                    case "pr:reviewer:needs_work":
                        await _pullRequestService.PullRequestReviewerStatusChangedAsync(eventData.ToObject<StashPullRequest>(), ReviewersStatus.Unapproved);
                        break;

                    case "pr:reviewer:updated":
                        await _pullRequestService.PullRequestReviewersUpdateAsync(eventData.ToObject<ReviewersChanged>());
                        break;

                    case "repo:refs_changed":
                        await _pushService.ChangesPushedAsync(eventData.ToObject<Push>());
                        break;

                    case "pr:comment:added":
                        await _commentService.AddOrEditCommentAsync(eventData.ToObject<CommentRequest>(), false);
                        break;

                    case "pr:comment:edited":
                        await _commentService.AddOrEditCommentAsync(eventData.ToObject<CommentRequest>(), true);
                        break;

                    case "pr:comment:deleted":
                        await _commentService.RemoveCommentAsync(eventData.ToObject<CommentRequest>());
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
            }
        }
    }
}