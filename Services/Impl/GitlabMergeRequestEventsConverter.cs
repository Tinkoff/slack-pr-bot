
using Newtonsoft.Json.Linq;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Gitlab.Events;
using SlackPrBot.Models.Stash;
using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Services.Impl
{
    internal class GitlabMergeRequestEventsConverter
    {
        public JObject Convert(MergeRequestEvent model)
        {
            var reviewerStatusChanged = ReviewerStatusChanged(model);
            if (reviewerStatusChanged != null) return reviewerStatusChanged;

            var statusChanged = PullStatusChanged(model);
            if (statusChanged != null) return statusChanged;

            var newCommit = PushNewCommit(model);
            if (newCommit != null) return newCommit;

            return PullRequestUpdated(model);
        }

        private JObject ReviewerStatusChanged(MergeRequestEvent model)
        {
            if (model.ObjectAttributes.Action != "approved" && model.ObjectAttributes.Action != "unapproved")
            {
                return null;
            }

            var converted = ConvertModelToStashPullRequest(model);

            switch (model.ObjectAttributes.Action)
            {
                case "approved":
                    converted.EventKey = "pr:reviewer:approved";
                    break;
                case "unapproved":
                    converted.EventKey = "pr:reviewer:unapproved";
                    break;
            }

            return JObject.FromObject(converted);
        }

        private JObject PullRequestUpdated(MergeRequestEvent model)
        {
            if (model.ObjectAttributes.Action != "update")
            {
                return null;
            }

            var converted = ConvertModelToStashPullRequest(model);
            converted.EventKey = "pr:modified";
            return JObject.FromObject(converted);
        }

        private JObject PullStatusChanged(MergeRequestEvent model)
        {
            if (model.ObjectAttributes.Action == "update")
            {
                return null;
            }
            var converted = ConvertModelToStashPullRequest(model);

            switch (model.ObjectAttributes.Action)
            {
                case "open":
                case "reopen":
                    converted.EventKey = "pr:opened";
                    break;
                case "merge":
                    converted.EventKey = "pr:merged";
                    break;
                case "close":
                    converted.EventKey = "pr:declined";
                    break;
            }

            return JObject.FromObject(converted);

        }

        private JObject PushNewCommit(MergeRequestEvent model)
        {
            if (model.ObjectAttributes.Action != "update" || model.ObjectAttributes.OldRev == null || model.ObjectAttributes.OldRev == model.ObjectAttributes.LastCommit.Id)
            {
                return null;
            }

            var converted = ConvertModelToStashPush(model);
            return JObject.FromObject(converted);
        }

        private Push ConvertModelToStashPush(MergeRequestEvent model)
        {
            var fromRepo = model.ObjectAttributes.ConvertModelToRepo(x => x.Source);
            var pushRef = new Ref { DisplayId = model.ObjectAttributes.SourceBranch, Repository = fromRepo };

            return new Push
            {
                Actor = model.User.ConvertGitlabUserToStashUser(),
                EventKey = "repo:refs_changed",
                Repository = fromRepo,
                Changes = new Change[] { new Change { Ref = pushRef, ToHash = model.ObjectAttributes.LastCommit.Id } }
            };
        }

        private StashPullRequest ConvertModelToStashPullRequest(MergeRequestEvent model)
        {
            var user = model.User.ConvertGitlabUserToStashUser();

            var pullRequest = model.ObjectAttributes.ConvertModelToPullRequest(model.User);

            var stashModel = new StashPullRequest
            {
                Actor = user,
                PullRequest = pullRequest
            };

            return stashModel;
        }
    }
}
