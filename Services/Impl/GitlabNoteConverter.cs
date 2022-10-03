
using Newtonsoft.Json.Linq;

using SlackPrBot.Extensions;
using SlackPrBot.Models.Gitlab.Events;
using SlackPrBot.Models.Stash;
using SlackPrBot.Models.Stash.Base;

namespace SlackPrBot.Services.Impl
{
    internal class GitlabNoteConverter
    {
        public JObject Convert(NoteEvent model)
        {
            var stashNote = new CommentRequest
            {
                Actor = model.User.ConvertGitlabUserToStashUser(),
                PullRequest = model.MergeRequest.ConvertModelToPullRequest(model.User),
                Comment = new Comment
                {
                    Id = model.ObjectAttributes.Id,
                    Text = model.ObjectAttributes.NoteText
                },
                EventKey = "pr:comment:added"
            };

            return JObject.FromObject(stashNote);
        }
    }
}
