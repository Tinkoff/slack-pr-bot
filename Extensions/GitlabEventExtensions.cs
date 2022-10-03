using System;

using SlackPrBot.Models.Gitlab.Events;
using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Extensions
{
    internal static class GitlabEventExtensions
    {
        public static Models.Stash.Base.User ConvertGitlabUserToStashUser(this Models.Gitlab.Events.User model)
        {
            return new Models.Stash.Base.User
            {
                DisplayName = model.Name,
                Name = model.UserName,
                EmailAddress = model.Email
            };
        }

        public static PullRequest ConvertModelToPullRequest(this MergeRequest mergeRequest, Models.Gitlab.Events.User user)
        {
            var userConverted = user.ConvertGitlabUserToStashUser();

            var fromRepo = mergeRequest.ConvertModelToRepo(x => mergeRequest.Source);

            var toRepo = mergeRequest.ConvertModelToRepo(x => mergeRequest.Target);

            return new PullRequest
            {
                Author = new Author { User = userConverted },
                Description = mergeRequest.Description,
                Id = mergeRequest.Id,
                Iid = mergeRequest.Iid,
                Title = mergeRequest.Title,
                FromRef = new Ref { DisplayId = mergeRequest.SourceBranch, Repository = fromRepo },
                ToRef = new Ref { DisplayId = mergeRequest.TargetBranch, Repository = toRepo }
            };
        }

        public static Repository ConvertModelToRepo(this MergeRequest model, Func<MergeRequest, RepoInfo> repoPredicate)
        {
            var repo = repoPredicate(model);
            return new Repository
            {
                Slug = repo.Slug,
                Project = new Project
                {
                    Key = repo.Key
                }
            };
        }

    }
}
